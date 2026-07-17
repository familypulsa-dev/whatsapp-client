using System;

namespace WaDesktop.Domain.Interfaces
{
    public interface ISplashView : IViewBase
    {
        event Action Initialized;
        void ShowStatus(string message, int percent);
        void CloseView();
        void ShowError(string message);
    }
}
