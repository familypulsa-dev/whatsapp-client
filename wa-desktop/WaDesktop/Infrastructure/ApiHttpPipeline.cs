using System.Net.Http;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Infrastructure.Data.Remote
{
    /// <summary>
    /// Satu-satunya tempat perakitan pipeline HttpClient (auth + refresh).
    /// Dipakai bersama oleh ApiClient lama dan seluruh DataSource baru
    /// agar semua jalur HTTP mendapat Bearer + refresh otomatis.
    /// </summary>
    public static class ApiHttpPipeline
    {
        public static HttpClient Create(IAuthSessionStore sessionStore, string baseUrl)
        {
            var handler = new Handlers.AuthDelegatingHandler(sessionStore, baseUrl)
            {
                InnerHandler = new HttpClientHandler()
            };
            return new HttpClient(handler);
        }
    }
}
