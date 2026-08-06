using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public Task<List<User>> GetUsersAsync()
            => GetListAsync<User>(ApiRoutes.Users.Base);

        public Task<User> CreateUserAsync(string username, string password, string name, string role, string companyId)
            => PostAsync<object, User>(ApiRoutes.Users.Base, new { username, password, name, role, company_id = companyId });

        public Task UpdateUserAsync(string id, string displayName, string role, string companyId, bool? isActive = null)
            => PutAsync($"{ApiRoutes.Users.Base}/{id}", new { name = displayName, role, company_id = companyId, is_active = isActive });

        public async Task DeactivateUserAsync(string id)
        {
            var res = await SendWithRefreshAsync(() =>
            {
                var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_baseUrl}{ApiRoutes.Users.Base}/{id}/deactivate");
                return _http.SendAsync(req);
            });
            await EnsureSuccessAsync(res);
        }

        public Task ResetPasswordAsync(string id, string newPassword)
            => PostAsync($"{ApiRoutes.Users.Base}/{id}/reset-password", new { new_password = newPassword });
    }
}