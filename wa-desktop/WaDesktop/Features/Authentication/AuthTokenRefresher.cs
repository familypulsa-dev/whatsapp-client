using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Domain.State;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public class AuthTokenRefresher : IAuthTokenRefresher, IDisposable
    {
        private readonly IAuthSessionStore _sessionStore;
        private readonly IPersistedSessionStore _persistedSessionStore;
        private readonly AppState _state;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        public AuthTokenRefresher(
            IAuthSessionStore sessionStore,
            IPersistedSessionStore persistedSessionStore,
            AppState state,
            string baseUrl)
        {
            _sessionStore = sessionStore;
            _persistedSessionStore = persistedSessionStore;
            _state = state;
            _baseUrl = baseUrl;
        }

        public async Task<bool> TryRefreshAsync()
        {
            var refreshToken = _sessionStore.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken)) return false;

            try
            {
                var json = JsonConvert.SerializeObject(new { refresh_token = refreshToken });
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var response = await _httpClient.PostAsync($"{_baseUrl}{ApiRoutes.Auth.Refresh}", content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == HttpStatusCode.BadRequest
                            || response.StatusCode == HttpStatusCode.Unauthorized
                            || response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            ClearInvalidSession();
                        }
                        return false;
                    }

                    var responseJson = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(responseJson)["data"];
                    var result = data != null ? data.ToObject<AuthResult>() : null;
                    if (result == null || string.IsNullOrEmpty(result.AccessToken))
                    {
                        ClearInvalidSession();
                        return false;
                    }

                    var newRefreshToken = string.IsNullOrEmpty(result.RefreshToken)
                        ? refreshToken
                        : result.RefreshToken;

                    _sessionStore.SetSession(result.AccessToken, newRefreshToken);
                    ApplyRefreshedState(result, newRefreshToken);
                    SaveCurrentSession();
                    _sessionStore.RaiseTokenRefreshed();
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch (JsonException)
            {
                ClearInvalidSession();
                return false;
            }
        }

        private void ApplyRefreshedState(AuthResult result, string refreshToken)
        {
            _state.AccessToken = result.AccessToken;
            _state.RefreshToken = refreshToken;

            if (result.User == null) return;

            _state.Role = result.User.Role;
            _state.DisplayName = result.User.DisplayName;
            _state.CompanyId = result.User.CompanyId;
            if (!string.IsNullOrEmpty(result.CompanyName)) _state.CompanyName = result.CompanyName;
        }

        private void SaveCurrentSession()
        {
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

        private void ClearInvalidSession()
        {
            _sessionStore.ClearSession();
            _state.ClearSession();
            _persistedSessionStore.Delete();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
