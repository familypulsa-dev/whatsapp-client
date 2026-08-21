using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var result = await PostAsync<object, AuthResult>(ApiRoutes.Auth.Login, new { username, password }, refresh: false);
            if (result != null)
                SetSession(result.AccessToken, result.RefreshToken);
            return result;
        }

        public async Task<bool> TryRefreshAsync()
        {
            if (string.IsNullOrEmpty(_refreshToken)) return false;

            try
            {
                using (var refreshHttp = new HttpClient())
                {
                    var body = JsonConvert.SerializeObject(new { refresh_token = _refreshToken });
                    var res = await refreshHttp.PostAsync($"{_baseUrl}{ApiRoutes.Auth.Refresh}",
                        new StringContent(body, Encoding.UTF8, "application/json"));
                    
                    if (!res.IsSuccessStatusCode) return false;

                    var json = await res.Content.ReadAsStringAsync();
                    var result = UnwrapData<AuthResult>(json);
                    
                    if (result == null || string.IsNullOrEmpty(result.AccessToken)) return false;

                    // Defensif: kalau respons refresh tidak membawa refresh_token baru
                    // (backend tidak rotasi), JANGAN nukul RT lama dengan null.
                    var newRefresh = string.IsNullOrEmpty(result.RefreshToken)
                        ? _refreshToken
                        : result.RefreshToken;

                    SetSession(result.AccessToken, newRefresh);
                    TokenRefreshed?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
