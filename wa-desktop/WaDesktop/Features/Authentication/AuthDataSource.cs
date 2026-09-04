using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk endpoint auth.</summary>
    /// <remarks>
    /// AuthResult dipakai langsung sebagai tipe payload karena memang DTO
    /// ber-atribut JsonProperty (paritas dengan unwrap legacy).
    /// </remarks>
    public class AuthDataSource : BaseDataSource
    {
        public AuthDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<AuthResult>> Login(string username, string password)
        {
            var payload = new { username, password };
            var result = await SendAsync(HttpMethod.Post, $"{BaseUrl}{ApiRoutes.Auth.Login}", payload);
            return result.IsSuccess
                ? ParseData<AuthResult>(result.Value)
                : Result<AuthResult>.Failure(result.Error);
        }
    }
}
