using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Users;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur user.</summary>
    public class UserDataSource : BaseDataSource
    {
        public UserDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<UserPayload>>> Fetch()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Users.Base}", null);
            return result.IsSuccess
                ? ParseList<UserPayload>(result.Value)
                : Result<List<UserPayload>>.Failure(result.Error);
        }

        public async Task<Result<UserPayload>> Create(string username, string password, string name, string role, string companyId)
        {
            var body = new { username, password, name, role, company_id = companyId };
            var result = await SendAsync(HttpMethod.Post, $"{BaseUrl}{ApiRoutes.Users.Base}", body);
            return result.IsSuccess
                ? ParseData<UserPayload>(result.Value)
                : Result<UserPayload>.Failure(result.Error);
        }

        /// <param name="isActive">Hanya masuk payload bila diisi.</param>
        /// <param name="isSuspend">Hanya masuk payload bila diisi.</param>
        public async Task<Result<bool>> Update(string id, string displayName, string role, string companyId, bool? isActive, bool? isSuspend)
        {
            var payload = new Newtonsoft.Json.Linq.JObject
            {
                ["name"] = displayName,
                ["role"] = role,
                ["company_id"] = companyId
            };
            if (isActive.HasValue) payload["is_active"] = isActive.Value;
            if (isSuspend.HasValue) payload["is_suspend"] = isSuspend.Value;

            var url = $"{BaseUrl}{ApiRoutes.Users.Base}/{id}";
            var result = await SendAsync(HttpMethod.Put, url, payload);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<bool>> Deactivate(string id)
        {
            var url = $"{BaseUrl}{ApiRoutes.Users.Base}/{id}/deactivate";
            var result = await SendAsync(new HttpMethod("PATCH"), url, null);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<bool>> ResetPassword(string id, string newPassword)
        {
            var url = $"{BaseUrl}{ApiRoutes.Users.Base}/{id}/reset-password";
            var result = await SendAsync(HttpMethod.Post, url, new { new_password = newPassword });
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }
    }
}
