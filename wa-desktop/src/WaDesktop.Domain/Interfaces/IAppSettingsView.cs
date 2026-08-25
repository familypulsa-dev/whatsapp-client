using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface IAppSettingsView : IViewBase
    {
        string WebhookBaseUrl { get; set; }
        bool MessageCleanupEnabled { get; set; }
        int MessageRetentionDays { get; set; }
        bool IsSaving { set; }

        event EventHandler SaveClicked;
        event EventHandler RefreshClicked;
        event EventHandler SetupWebhookClicked;

        void ShowSuccess(string message);
        void ShowWarning(string message);
        void ShowError(string message);
    }
}
