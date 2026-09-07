using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Data.Remote.Handlers
{
    /// <summary>
    /// Pipeline HTTP: injeksi Bearer, refresh token sekali (single-flight)
    /// saat 401, lalu retry permintaan. Port dari logika lama
    /// ApiClient.SendWithRefreshAsync + TryRefreshAsync.
    ///
    /// Guard keamanan: Bearer dan refresh-retry HANYA untuk host API.
    /// Permintaan eksternal (mis. avatar Meta CDN) tidak disentuh.
    /// </summary>
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly IAuthSessionStore _store;
        private readonly IAuthTokenRefresher _tokenRefresher;
        private readonly string _baseUrl;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        public AuthDelegatingHandler(
            IAuthSessionStore store,
            IAuthTokenRefresher tokenRefresher,
            string baseUrl)
        {
            _store = store;
            _tokenRefresher = tokenRefresher;
            _baseUrl = baseUrl;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isApiHost = IsApiHost(request.RequestUri);
            var isAuthEndpoint = isApiHost && IsAuthEndpoint(request.RequestUri);

            if (isApiHost && !isAuthEndpoint && !string.IsNullOrEmpty(_store.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _store.AccessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (!isApiHost || isAuthEndpoint || response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return response;

            bool refreshed = false;
            var initialToken = _store.AccessToken;

            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                // Jika token sudah berubah saat menunggu lock, thread lain sudah refresh.
                refreshed = (!string.IsNullOrEmpty(_store.AccessToken) && initialToken != _store.AccessToken)
                    || await _tokenRefresher.TryRefreshAsync();
            }
            finally
            {
                _refreshLock.Release();
            }

            if (!refreshed)
            {
                _store.RaiseSessionExpired();
                throw new HttpRequestException("Session expired");
            }

            var retry = await CloneRequestAsync(request);
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _store.AccessToken);
            response.Dispose();

            response = await base.SendAsync(retry, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _store.RaiseSessionExpired();
                throw new HttpRequestException("Session expired");
            }

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshLock.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri) { Version = request.Version };

            if (request.Content != null)
            {
                var bytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(bytes);
                foreach (var h in request.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }

            foreach (var h in request.Headers)
                clone.Headers.TryAddWithoutValidation(h.Key, h.Value);

            return clone;
        }

        private bool IsApiHost(Uri uri)
        {
            if (uri == null) return false;
            var apiUri = new Uri(_baseUrl);
            return string.Equals(uri.Host, apiUri.Host, StringComparison.OrdinalIgnoreCase)
                && uri.Port == apiUri.Port;
        }

        private static bool IsAuthEndpoint(Uri uri)
        {
            var path = uri.AbsolutePath ?? string.Empty;
            return path.EndsWith(ApiRoutes.Auth.Login) || path.EndsWith(ApiRoutes.Auth.Refresh);
        }
    }
}
