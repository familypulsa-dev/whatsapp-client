using System.Threading.Tasks;
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

        // TryRefreshAsync dipindahkan ke Data/Remote/Handlers/AuthDelegatingHandler (Fase 2).
    }
}
