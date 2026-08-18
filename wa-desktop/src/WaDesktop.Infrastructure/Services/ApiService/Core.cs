using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient : IApiClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private string _accessToken;
        private string _refreshToken;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        public event EventHandler SessionExpired;
        public event EventHandler TokenRefreshed;

        public string AccessToken => _accessToken;
        public string RefreshToken => _refreshToken;

        public ApiClient(string baseUrl = "http://localhost:8080")
        {
            _baseUrl = baseUrl;
            _http = new HttpClient();
        }

        public void SetToken(string token)
        {
            _accessToken = token;
            _http.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null : new AuthenticationHeaderValue("Bearer", token);
        }

        public void SetSession(string accessToken, string refreshToken)
        {
            SetToken(accessToken);
            _refreshToken = refreshToken;
        }

        // ── Internal HTTP Helpers ──

        private async Task<T> GetAsync<T>(string path)
        {
            var json = await GetStringAsync(path);
            return UnwrapData<T>(json);
        }

        private async Task<List<T>> GetListAsync<T>(string path)
        {
            var json = await GetStringAsync(path);
            var wrapper = JsonConvert.DeserializeObject<ApiListResponse<T>>(json);
            return wrapper?.Data ?? new List<T>();
        }

        private async Task<string> GetStringAsync(string path)
        {
            var res = await SendWithRefreshAsync(() => _http.GetAsync($"{_baseUrl}{path}"));
            await EnsureSuccessAsync(res);
            return await res.Content.ReadAsStringAsync();
        }

        private async Task PostAsync<TReq>(string path, TReq req, bool refresh = true)
        {
            var res = await SendContentAsync(HttpMethod.Post, path, req, refresh);
            await EnsureSuccessAsync(res);
        }

        private async Task<TRes> PostAsync<TReq, TRes>(string path, TReq req, bool refresh = true)
        {
            var res = await SendContentAsync(HttpMethod.Post, path, req, refresh);
            await EnsureSuccessAsync(res);
            var json = await res.Content.ReadAsStringAsync();
            return UnwrapData<TRes>(json);
        }

        private async Task PutAsync<TReq>(string path, TReq req)
        {
            var res = await SendContentAsync(HttpMethod.Put, path, req, true);
            await EnsureSuccessAsync(res);
        }

        private async Task<TRes> PutAsync<TReq, TRes>(string path, TReq req)
        {
            var res = await SendContentAsync(HttpMethod.Put, path, req, true);
            await EnsureSuccessAsync(res);
            var json = await res.Content.ReadAsStringAsync();
            return UnwrapData<TRes>(json);
        }

        private async Task DeleteRequestAsync(string path)
        {
            var res = await SendWithRefreshAsync(() => _http.DeleteAsync($"{_baseUrl}{path}"));
            await EnsureSuccessAsync(res);
        }

        private Task<HttpResponseMessage> SendContentAsync<TReq>(HttpMethod method, string path, TReq req, bool refresh)
        {
            Func<Task<HttpResponseMessage>> sendFunc = () =>
            {
                var request = new HttpRequestMessage(method, $"{_baseUrl}{path}");
                if (req != null)
                {
                    var body = JsonConvert.SerializeObject(req);
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
                return _http.SendAsync(request);
            };

            return refresh ? SendWithRefreshAsync(sendFunc) : sendFunc();
        }

        private async Task<HttpResponseMessage> SendWithRefreshAsync(Func<Task<HttpResponseMessage>> send)
        {
            var initialToken = _accessToken;
            var res = await send();
            
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                bool refreshed = false;
                await _refreshLock.WaitAsync();
                try
                {
                    // Jika token sudah berubah saat menunggu lock, berarti thread lain sudah refresh
                    if (initialToken != _accessToken)
                    {
                        refreshed = true;
                    }
                    else
                    {
                        refreshed = await TryRefreshAsync();
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }

                if (refreshed)
                {
                    res = await send();
                    if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        SessionExpired?.Invoke(this, EventArgs.Empty);
                        throw new HttpRequestException("Session expired");
                    }
                }
                else
                {
                    // Pastikan event ditembak kalau refresh gagal!
                    SessionExpired?.Invoke(this, EventArgs.Empty);
                    throw new HttpRequestException("Session expired");
                }
            }
            return res;
        }

        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var errContent = await response.Content.ReadAsStringAsync();
            string message = null;
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errContent);
                message = errorResponse?.GetErrorMessage();
            }
            catch { /* Abaikan jika respons bukan JSON (misalnya HTML 500 dari server) */ }

            throw new HttpRequestException(message ?? $"Request failed ({response.StatusCode}): {errContent}");
        }

        private static T UnwrapData<T>(string json)
        {
            var wrapper = JObject.Parse(json);
            var data = wrapper["data"];
            return data != null ? data.ToObject<T>() : default;
        }

        private class ApiListResponse<T>
        {
            [JsonProperty("data")] public List<T> Data { get; set; }
        }

        private class ApiErrorResponse
        {
            [JsonProperty("message")] public string Message { get; set; }
            [JsonProperty("error")] public JToken Error { get; set; }
            [JsonProperty("errors")] public JToken Errors { get; set; }

            public string GetErrorMessage()
            {
                var token = Error ?? Errors;
                if (token != null)
                {
                    if (token.Type == JTokenType.String)
                        return token.ToString();

                    if (token.Type == JTokenType.Object)
                    {
                        var obj = token.ToObject<ApiErrorDetail>();
                        return obj?.Message;
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        var arr = token.ToObject<List<ApiErrorDetail>>();
                        if (arr != null && arr.Count > 0)
                        {
                            var messages = arr.Where(x => !string.IsNullOrEmpty(x.Message))
                                              .Select(x => string.IsNullOrEmpty(x.Field) ? x.Message : $"{x.Field}: {x.Message}");
                            return string.Join("\n", messages);
                        }
                    }
                }
                return Message;
            }
        }

        private class ApiErrorDetail
        {
            [JsonProperty("field")] public string Field { get; set; }
            [JsonProperty("message")] public string Message { get; set; }
        }
    }
}