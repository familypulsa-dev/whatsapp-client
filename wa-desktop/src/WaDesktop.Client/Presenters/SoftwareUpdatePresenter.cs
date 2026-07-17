using System;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Client.Presenters
{
    public class SoftwareUpdatePresenter : IDisposable
    {
        private readonly ISoftwareUpdateView _view;
        private readonly IUpdateService _updateService;
        private bool _disposed;

        public SoftwareUpdatePresenter(ISoftwareUpdateView view, IUpdateService updateService)
        {
            _view = view;
            _updateService = updateService;

            _view.OnLoadView += HandleLoadView;
            _view.OnClickDownloadUpdate += HandleDownloadUpdate;
            _view.OnClickClose += HandleClose;
        }

        private async void HandleLoadView()
        {
            try
            {
                var info = await _updateService.CheckForUpdatesAsync();
                if (info != null)
                {
                    _view.SetParameters(info.Version, info.ReleaseNotes ?? "Tidak ada catatan rilis.");
                    _view.SetCanUpdate(true);
                }
                else
                {
                    _view.SetCanUpdate(false);
                }
            }
            catch (Exception ex)
            {
                _view.SetCanUpdate(false);
                _view.ShowError("Gagal memeriksa pembaruan: " + ex.Message);
            }
        }

        private async void HandleDownloadUpdate()
        {
            try
            {
                var progress = new Progress<(string status, int percent)>(p =>
                {
                    _view.UpdateProgress(p.status, p.percent);
                });

                await _updateService.DownloadAndApplyUpdateAsync(progress);
            }
            catch (Exception ex)
            {
                _view.ShowError("Gagal mengunduh pembaruan: " + ex.Message);
            }
        }

        private void HandleClose()
        {
            _view.CloseView();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _view.OnLoadView -= HandleLoadView;
                _view.OnClickDownloadUpdate -= HandleDownloadUpdate;
                _view.OnClickClose -= HandleClose;
                _disposed = true;
            }
        }
    }
}
