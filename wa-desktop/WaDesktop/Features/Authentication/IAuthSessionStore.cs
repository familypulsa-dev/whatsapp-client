using System;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>
    /// Sumber kebenaran tunggal untuk sesi token. Dikonsumsi oleh
    /// AuthDelegatingHandler (pipeline HTTP) dan diekspos ke lapisan
    /// presentasi melalui event SessionExpired/TokenRefreshed.
    /// </summary>
    public interface IAuthSessionStore
    {
        string AccessToken { get; }
        string RefreshToken { get; }

        void SetSession(string accessToken, string refreshToken);
        void ClearAccessToken();

        void RaiseSessionExpired();
        void RaiseTokenRefreshed();

        event EventHandler SessionExpired;
        event EventHandler TokenRefreshed;
    }
}
