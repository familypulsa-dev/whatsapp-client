using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class SplashPresenter : IDisposable
    {
        private readonly ISplashView _view;
        private readonly IUpdateService _updateService;
        private bool _disposed;

        public SplashPresenter(ISplashView view, IUpdateService updateService)
        {
            _view = view;
            _updateService = updateService;

            _view.Initialized += OnInitialized;
        }

        private async void OnInitialized()
        {
            try
            {
                var progress = new Progress<(string status, int percent)>(p =>
                {
                    _view.ShowStatus(p.status, p.percent);
                });

                await _updateService.CheckAndDownloadUpdateAsync(progress);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error update: {ex.Message}");
            }
            finally
            {
                // Kasih delay sebentar biar tulisan 100% / sukses terbaca sebelum close
                await Task.Delay(500);
                _view.CloseView();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.Initialized -= OnInitialized;
                _disposed = true;
            }
        }
    }
}
