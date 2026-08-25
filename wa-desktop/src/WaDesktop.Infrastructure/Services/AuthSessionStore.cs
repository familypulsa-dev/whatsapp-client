using System;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Infrastructure.Services
{
    /// <summary>Holder token sesi. Tidak tahu HTTP — murni state + event.</summary>
    public class AuthSessionStore : IAuthSessionStore
    {
        private readonly object _gate = new object();
        private string _accessToken;
        private string _refreshToken;

        public event EventHandler SessionExpired;
        public event EventHandler TokenRefreshed;

        public string AccessToken
        {
            get { lock (_gate) { return _accessToken; } }
        }

        public string RefreshToken
        {
            get { lock (_gate) { return _refreshToken; } }
        }

        public void SetSession(string accessToken, string refreshToken)
        {
            lock (_gate)
            {
                _accessToken = accessToken;
                _refreshToken = refreshToken;
            }
        }

        public void ClearAccessToken()
        {
            lock (_gate)
            {
                _accessToken = null;
            }
        }

        public void RaiseSessionExpired()
        {
            SessionExpired?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseTokenRefreshed()
        {
            TokenRefreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
