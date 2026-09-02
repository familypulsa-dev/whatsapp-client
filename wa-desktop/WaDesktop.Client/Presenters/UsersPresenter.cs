using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Client.Views.ManagementViews;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;

namespace WaDesktop.Client.Presenters
{
    public class UsersPresenter : IDisposable, IPresenterBase
    {
        private readonly UsersView _view;
        private readonly IUserRepository _users;
        private readonly ICompanyRepository _companies;
        private bool _disposed;
        private const string DefaultPassword = "WaClientDefault123?";
        private readonly AppState _state;

        public UsersPresenter(UsersView view, IUserRepository users, ICompanyRepository companies, AppState state)
        {
            _view = view;
            _users = users;
            _companies = companies;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.SaveClicked += OnSaveClicked;
            _view.ResetPasswordClicked += OnResetPassword;
            _state = state;
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            _view.IsLoading = true;
            try
            {
                var companiesResult = await Task.Run(() => _companies.GetAllAsync());
                if (companiesResult.IsFailure)
                    throw new Exception(companiesResult.Error.Message);
                if (!_state.IsSuperAdmin)
                {
                    // Filter companies based on the user's company ID
                    companiesResult = Result<List<Company>>.Success(
                        companiesResult.Value.Where(c => c.Id == _state.CompanyId).ToList()
                    );
                }
                _view.SetCompanies(companiesResult.Value);

                var result = await Task.Run(() => _users.GetAllAsync());
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);

                var data = result.Value;
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(u =>
                        (u.DisplayName?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (u.Username?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    ).ToList();
                }
                _view.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _view.IsLoading = true;
            try
            {
                foreach (string id in _view.GetDeletedIds())
                {
                    var del = await Task.Run(() => _users.DeactivateAsync(id));
                    if (del.IsFailure)
                        throw new Exception(del.Error.Message);
                }

                foreach (User u in _view.GetModifiedRows())
                {
                    Result<bool> result;
                    if (string.IsNullOrEmpty(u.Id))
                    {
                        // User Baru: Jika DgvPassword kosong, gunakan DefaultPassword
                        string pw = string.IsNullOrEmpty(u.NewPassword) ? DefaultPassword : u.NewPassword;
                        var created = await Task.Run(() => _users.CreateAsync(u.Username, pw, u.DisplayName, u.Role, u.CompanyId));
                        result = created.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(created.Error);
                    }
                    else
                    {
                        // User Lama: Update data biasa
                        result = await Task.Run(() => _users.UpdateAsync(u.Id, u.DisplayName, u.Role, u.CompanyId, u.IsActive, u.IsSuspend));

                        // Eksekusi ganti password jika ada inputan di DgvPassword
                        if (result.IsSuccess && !string.IsNullOrEmpty(u.NewPassword))
                        {
                            result = await Task.Run(() => _users.ResetPasswordAsync(u.Id, u.NewPassword));
                        }
                    }

                    if (result.IsFailure)
                        throw new Exception(result.Error.Message);
                }

                await LoadDataAsync();
                MessageBox.Show("Data berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnResetPassword(object sender, string userId)
        {
            var confirm = MessageBox.Show("Reset password user ini menjadi " + DefaultPassword + "?",
                "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            _view.IsLoading = true;
            try
            {
                var result = await Task.Run(() => _users.ResetPasswordAsync(userId, DefaultPassword));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);

                MessageBox.Show($"Password berhasil direset menjadi {DefaultPassword}",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal reset password: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.RefreshClicked -= null;
                _view.SearchClicked -= null;
                _view.SaveClicked -= null;
                _view.ResetPasswordClicked -= null;
                _disposed = true;
            }
        }
    }
}
