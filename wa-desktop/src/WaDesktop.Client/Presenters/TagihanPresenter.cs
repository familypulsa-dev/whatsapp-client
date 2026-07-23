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
        private readonly IApiClient _api;
        private bool _disposed;
        private bool _wabasLoaded;

        public TagihanPresenter(TagihanView view, IApiClient api)
        {
            _view = view;
            _api = api;
            _view.FilterClicked += OnFilterClicked;
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
                    var wabas = await Task.Run(() => _api.GetWabasAsync());
                    _view.SetWabaDataSource(wabas);
                    _wabasLoaded = true;
                }

                var start = _view.FilterStart;
                var end = _view.FilterEnd;
                var wabaId = _view.SelectedWabaId;

                var data = await Task.Run(() =>
                    _api.GetBillingSummaryAsync(start, end, wabaId));
                _view.DataSource = data;
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

        private async void OnWabaFilterChanged(object sender, string wabaId)
        {
            await LoadDataAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.FilterClicked -= OnFilterClicked;
                _view.WabaFilterChanged -= OnWabaFilterChanged;
                _disposed = true;
            }
        }
    }
}
