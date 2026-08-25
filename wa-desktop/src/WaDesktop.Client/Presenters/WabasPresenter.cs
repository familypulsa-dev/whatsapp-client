using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Client.Views.ManagementViews;
using WaDesktop.Client.Views.ManagementViews;

namespace WaDesktop.Client.Presenters
{
    public class WabasPresenter : IDisposable, IPresenterBase
    {
        private readonly WabaView _view;
        private readonly IWabaRepository _wabas;
        private bool _disposed;

        public WabasPresenter(WabaView view, IWabaRepository wabas)
        {
            _view = view;
            _wabas = wabas;

            _view.RefreshClicked += async (s, e) => await LoadDataAsync();
            _view.SearchClicked += async (s, q) => await LoadDataAsync(q);
            _view.AddClicked += OnAdd;
            _view.EditClicked += OnEdit;
            _view.DeleteClicked += OnDelete;
            _view.SyncClicked += OnSync;
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            _view.IsLoading = true;
            try
            {
                var wabasResult = await Task.Run(() => _wabas.GetAllAsync());
                if (wabasResult.IsFailure)
                    throw new Exception(wabasResult.Error.Message);

                var wabas = wabasResult.Value;

                if (!string.IsNullOrEmpty(search))
                    wabas = wabas.FindAll(w =>
                        (w.Name ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (w.WabaId ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);

                _view.DataSource = wabas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load WABA: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private void OnAdd(object sender, EventArgs e)
        {
            MessageBox.Show("Tambah WABA — implement form dialog.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSync(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Sinkronisasi WABA dari Meta?", "Sinkronisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _view.IsLoading = true;
                Task.Run(async () =>
                {
                    try
                    {
                        var sync = await _wabas.SyncFromMetaAsync();
                        if (sync.IsFailure)
                            throw new Exception(sync.Error.Message);
                        await LoadDataAsync();
                        MessageBox.Show("Sinkronisasi selesai.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Sinkronisasi gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        _view.IsLoading = false;
                    }
                });
            }
        }

        private void OnEdit(object sender, string wabaId)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            MessageBox.Show("Edit WABA — implement form dialog.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnDelete(object sender, EventArgs e)
        {
            if (_view.SelectedIndex < 0) { MessageBox.Show("Pilih baris dulu.", "Info"); return; }
            MessageBox.Show("Delete WABA — implement API call if needed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.RefreshClicked -= null;
                _view.SearchClicked -= null;
                _view.AddClicked -= null;
                _view.EditClicked -= null;
                _view.DeleteClicked -= null;
                _view.SyncClicked -= null;
                _disposed = true;
            }
        }
    }
}