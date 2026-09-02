using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaDesktop.Client.Views.ManagementViews;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class TagihanPresenter : IDisposable, IPresenterBase
    {
        private readonly TagihanView _view;
        private readonly IWabaRepository _wabas;
        private readonly IBillingRepository _billing;
        private bool _disposed;
        private bool _wabasLoaded;

        public TagihanPresenter(TagihanView view, IWabaRepository wabas, IBillingRepository billing)
        {
            _view = view;
            _wabas = wabas;
            _billing = billing;
            _view.FilterClicked += OnFilterClicked;
            _view.SyncClicked += OnSyncClicked;
            _view.RefreshClicked += OnRefreshClicked;
            _view.WabaFilterChanged += OnWabaFilterChanged;
        }

        public async void LoadData() => await LoadDataAsync();

        public async void LoadData(string search)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _view.IsLoading = true;
            try
            {
                if (!_wabasLoaded)
                {
                    var wabasResult = await Task.Run(() => _wabas.GetAllAsync());
                    if (wabasResult.IsFailure)
                        throw new Exception(wabasResult.Error.Message);
                    _view.SetWabaDataSource(wabasResult.Value);
                    _wabasLoaded = true;
                }

                var start = _view.FilterStart;
                var end = _view.FilterEnd;
                var wabaId = _view.SelectedWabaId;

                var result = await Task.Run(() =>
                    _billing.GetUsageSummaryAsync(start, end, wabaId));
                if (result.IsFailure)
                    throw new Exception(result.Error.Message);
                _view.DataSource = result.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal load tagihan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnFilterClicked(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private void OnSyncClicked(object sender, EventArgs e)
        {
            var start = _view.FilterStart;
            var end = _view.FilterEnd;
            var startStr = start.ToString("yyyy-MM-dd");
            var endStr = end.ToString("yyyy-MM-dd");
            var wabaId = _view.SelectedWabaId;

            var msgPrompt = string.IsNullOrEmpty(wabaId)
                ? $"Tarik & sinkronkan data tagihan percakapan dari Meta untuk periode {startStr} s/d {endStr}?"
                : $"Tarik & sinkronkan data tagihan percakapan dari Meta untuk WABA terpilih (periode {startStr} s/d {endStr})?";

            var confirm = MessageBox.Show(
                msgPrompt,
                "Sinkronisasi Tagihan Meta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _view.IsLoading = true;
            Task.Run(async () =>
            {
                try
                {
                    var syncResult = await _billing.SyncBillingAsync(startStr, endStr, wabaId);
                    if (syncResult.IsFailure)
                        throw new Exception(syncResult.Error.Message);

                    await LoadDataAsync();
                    MessageBox.Show("Sinkronisasi data tagihan dari Meta berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Sinkronisasi tagihan gagal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _view.IsLoading = false;
                }
            });
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            _wabasLoaded = false;
            await LoadDataAsync();
        }

        private async void OnWabaFilterChanged(object sender, string wabaId)
        {
            await LoadDataAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.FilterClicked -= OnFilterClicked;
                _view.SyncClicked -= OnSyncClicked;
                _view.RefreshClicked -= OnRefreshClicked;
                _view.WabaFilterChanged -= OnWabaFilterChanged;
                _disposed = true;
            }
        }
    }
}
