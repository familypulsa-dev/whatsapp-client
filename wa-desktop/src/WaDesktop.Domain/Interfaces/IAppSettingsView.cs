using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface IAppSettingsView : IViewBase
    {
        string WabaToken { get; set; }
        string AppId { get; set; }
        string BusinessId { get; set; }
        string VerifyToken { get; set; }
        string WebhookBaseUrl { get; set; }
        bool IsSaving { set; }

        event EventHandler SaveClicked;
        event EventHandler RefreshClicked;
        event EventHandler SetupWebhookClicked;

        void ShowSuccess(string message);
        void ShowWarning(string message);
        void ShowError(string message);
    }
}
