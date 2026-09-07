using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;

namespace WaDesktop.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IAuthSessionStore _sessionStore;
        private readonly IPersistedSessionStore _persistedSessionStore;
        private readonly IAuthTokenRefresher _tokenRefresher;
        private readonly AppState _state;

        public AuthService(
            IAuthRepository authRepository,
            IAuthSessionStore sessionStore,
            IPersistedSessionStore persistedSessionStore,
            IAuthTokenRefresher tokenRefresher,
            AppState state)
        {
            _authRepository = authRepository;
            _sessionStore = sessionStore;
            _persistedSessionStore = persistedSessionStore;
            _tokenRefresher = tokenRefresher;
            _state = state;

            // Sync AppState saat handler refresh token di pipeline HTTP.
            _sessionStore.TokenRefreshed += (s, e) =>
            {
                _state.AccessToken = _sessionStore.AccessToken;
                _state.RefreshToken = _sessionStore.RefreshToken;
            };
        }

        public string AccessToken => _state.AccessToken;
        public string RefreshToken => _state.RefreshToken;
        public string Role => _state.Role;
        public string DisplayName => _state.DisplayName;
        public bool IsLoggedIn => _state.IsLoggedIn;
        public bool IsSuperAdmin => _state.IsSuperAdmin;

        public async Task<(bool, string)> LoginAsync(string username, string password)
        {
            var result = await Task.Run(() => _authRepository.LoginAsync(username, password));
            if (result.IsFailure)
                if (result.Error != null) { return (false, result.Error.Message); }else { return (false, "Login failed"); }

            var auth = result.Value;
            _sessionStore.SetSession(auth.AccessToken, auth.RefreshToken);
            _state.SetSession(auth.AccessToken, auth.RefreshToken, auth.User.Role, auth.User.DisplayName, auth.CompanyName, auth.User.CompanyId);
            SaveCurrentSession();
            return (true, null);
        }

        public async Task<bool> RestoreSessionAsync()
        {
            var persisted = _persistedSessionStore.Load();
            if (persisted == null) return false;

            _sessionStore.SetSession(persisted.AccessToken, persisted.RefreshToken);
            _state.SetSession(
                persisted.AccessToken,
                persisted.RefreshToken,
                persisted.Role,
                persisted.DisplayName,
                persisted.CompanyName,
                persisted.CompanyId);

            if (await _tokenRefresher.TryRefreshAsync()) return true;

            // Keep the encrypted snapshot for transient network failures, but do not
            // expose an unvalidated session to the application in this process.
            _sessionStore.ClearSession();
            _state.ClearSession();
            return false;
        }

        public Task<bool> RefreshTokenAsync() => _tokenRefresher.TryRefreshAsync();

        public void Logout()
        {
            _state.ClearSession();
            _sessionStore.ClearSession();
            _persistedSessionStore.Delete();
        }

        private void SaveCurrentSession()
        {
            if (string.IsNullOrEmpty(_sessionStore.RefreshToken)) return;

            _persistedSessionStore.Save(new PersistedSession
            {
                AccessToken = _sessionStore.AccessToken,
                RefreshToken = _sessionStore.RefreshToken,
                Role = _state.Role,
                DisplayName = _state.DisplayName,
                CompanyName = _state.CompanyName,
                CompanyId = _state.CompanyId
            });
        }
    }
}
