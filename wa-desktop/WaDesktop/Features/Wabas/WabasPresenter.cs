using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.State;
using WaDesktop.Client.Views.ManagementViews;

namespace WaDesktop.Client.Presenters
{
    public class WabasPresenter : IDisposable, IPresenterBase
    {
        private readonly WabaView _view;
        private readonly IWabaRepository _wabas;
        private bool _disposed;
        private readonly AppState _state;
        private bool _busy;

        public WabasPresenter(WabaView view, IWabaRepository wabas, AppState state)
        {
            _view = view;
            _wabas = wabas;
            _state = state;

            _view.RefreshClicked += OnRefresh;
            _view.SearchClicked += OnSearch;
            _view.EditClicked += OnEdit;
            _view.DeleteClicked += OnDelete;
            _view.SyncClicked += OnSync;
        }

        public async void LoadData(string search = null) => await LoadDataAsync(search);

        private async Task LoadDataAsync(string search = null)
        {
            if (_busy || _disposed) return;
            _busy = true;
            _view.IsLoading = true;
            try
            {
                var wabasResult = await _wabas.GetAllAsync();
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
                _busy = false;
            }
        }

        private async void OnRefresh(object sender, EventArgs e) => await LoadDataAsync();
        private async void OnSearch(object sender, string search) => await LoadDataAsync(search);

        private async void OnSync(object sender, EventArgs e)
        {
            if (_busy || _disposed) return;
            var result = MessageBox.Show("Sinkronisasi WABA dari Meta?", "Sinkronisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _view.IsLoading = true;
                _busy = true;
                try
                {
                    var sync = await _wabas.SyncFromMetaAsync();
                    if (sync.IsFailure)
                        throw new Exception(sync.Error.Message);
                    var list = await _wabas.GetAllAsync();
                    if (list.IsFailure) throw new Exception(list.Error.Message);
                    _view.DataSource = list.Value;
                    MessageBox.Show("Sinkronisasi selesai.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Sinkronisasi gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _view.IsLoading = false;
                    _busy = false;
                }
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
                _view.RefreshClicked -= OnRefresh;
                _view.SearchClicked -= OnSearch;
                _view.EditClicked -= OnEdit;
                _view.DeleteClicked -= OnDelete;
                _view.SyncClicked -= OnSync;
                _disposed = true;
            }
        }
    }
}
