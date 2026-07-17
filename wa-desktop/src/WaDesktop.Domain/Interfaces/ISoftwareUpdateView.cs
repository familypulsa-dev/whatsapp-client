using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface ISoftwareUpdateView : IViewBase
    {
        void SetParameters(string newVersion, string notes);
        void SetCanUpdate(bool canUpdate);
        void UpdateProgress(string status, int percent);
        void CloseView();
        void ShowError(string message);
        event Action OnClickDownloadUpdate;
        event Action OnClickClose;
        event Action OnLoadView;
    }
}
