using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Views.ManagementViews;

namespace WaDesktop.Client.Presenters
{
    public class CompanyPresenter : IDisposable, IPresenterBase
    {
        private readonly CompanyView _view;
        private readonly ICompanyRepository _companies;
        private bool _disposed;

        public CompanyPresenter(CompanyView view, ICompanyRepository companies)
        {
            _view = view;
            _companies = companies;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.SaveClicked += OnSaveClicked;
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            _view.IsLoading = true;
            try
            {
                var result = await Task.Run(() => _companies.GetAllAsync());
                if (result.IsFailure)
                {
                    MessageBox.Show("Gagal load companies: " + result.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var data = result.Value;
                if (!string.IsNullOrEmpty(search))
                    data = data.Where(c => c.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                _view.DataSource = data;
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
                // Delete rows that user removed via Delete key
                foreach (string id in _view.GetDeletedIds())
                {
                    var del = await Task.Run(() => _companies.DeleteAsync(id));
                    if (del.IsFailure)
                        throw new Exception(del.Error.Message);
                }

                // Create new / update existing
                foreach (Company c in _view.GetModifiedRows())
                {
                    Result<Company> result;
                    if (string.IsNullOrEmpty(c.Id))
                        result = await Task.Run(() => _companies.CreateAsync(c.Name));
                    else
                        result = await Task.Run(() => _companies.UpdateAsync(c.Id, c.Name));

                    if (result.IsFailure)
                        throw new Exception(result.Error.Message);
                }

                await LoadDataAsync();
                MessageBox.Show("Data berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _disposed = true;
            }
        }
    }
}
