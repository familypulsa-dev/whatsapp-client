using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;

namespace WaDesktop.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IAuthSessionStore _sessionStore;
        private readonly AppState _state;

        public AuthService(IAuthRepository authRepository, IAuthSessionStore sessionStore, AppState state)
        {
            _authRepository = authRepository;
            _sessionStore = sessionStore;
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

        public async Task<bool> LoginAsync(string username, string password)
        {
            var result = await Task.Run(() => _authRepository.LoginAsync(username, password));
            if (result.IsFailure)
                return false;

            var auth = result.Value;
            _sessionStore.SetSession(auth.AccessToken, auth.RefreshToken);
            _state.SetSession(auth.AccessToken, auth.RefreshToken, auth.User.Role, auth.User.DisplayName);
            _state.CompanyName = auth.CompanyName;
            return true;
        }

        public Task<bool> RefreshTokenAsync()
        {
            // Ditangani AuthDelegatingHandler (single-flight + retry-once).
            return Task.FromResult(false);
        }

        public void Logout()
        {
            _state.ClearSession();
            _sessionStore.ClearAccessToken();
        }
    }
}
