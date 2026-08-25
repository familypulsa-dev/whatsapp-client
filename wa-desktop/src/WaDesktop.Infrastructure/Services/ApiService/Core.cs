using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private readonly IAuthSessionStore _sessionStore;

        public event EventHandler SessionExpired;
        public event EventHandler TokenRefreshed;

        public string AccessToken => _sessionStore.AccessToken;
        public string RefreshToken => _sessionStore.RefreshToken;

        public ApiClient(string baseUrl = "http://localhost:8080", IAuthSessionStore sessionStore = null, HttpClient http = null)
        {
            _baseUrl = baseUrl;
            _sessionStore = sessionStore ?? new AuthSessionStore();

            // Auth (Bearer + refresh-retry) ditangani pipeline, bukan lagi manual.
            _http = http ?? Data.Remote.ApiHttpPipeline.Create(_sessionStore, baseUrl);

            // Facade event: konsumen lama (Program.cs bridge, AuthService) tetap jalan tanpa ubah kode.
            _sessionStore.SessionExpired += (s, e) => SessionExpired?.Invoke(this, e);
            _sessionStore.TokenRefreshed += (s, e) => TokenRefreshed?.Invoke(this, e);
        }

        public void SetToken(string token)
        {
            _sessionStore.SetSession(token, _sessionStore.RefreshToken);
        }

        public void SetSession(string accessToken, string refreshToken)
        {
            _sessionStore.SetSession(accessToken, refreshToken);
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

        private Task<HttpResponseMessage> SendWithRefreshAsync(Func<Task<HttpResponseMessage>> send)
        {
            // Refresh + retry saat 401 kini ditangani AuthDelegatingHandler di pipeline.
            return send();
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