using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Wabas;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur WABA.</summary>
    public class WabaDataSource : BaseDataSource
    {
        public WabaDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<WabaPayload>>> Fetch()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Waba.Base}", null);
            return result.IsSuccess
                ? ParseList<WabaPayload>(result.Value)
                : Result<List<WabaPayload>>.Failure(result.Error);
        }

        public async Task<Result<bool>> UpdateCompany(string wabaId, string companyId)
        {
            var url = $"{BaseUrl}{ApiRoutes.Waba.Base}/{wabaId}";
            var result = await SendAsync(HttpMethod.Put, url, new { company_id = companyId });
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<bool>> SyncFromMeta()
        {
            var url = $"{BaseUrl}{ApiRoutes.Waba.Base}/sync";
            var result = await SendAsync(HttpMethod.Post, url, null);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }
    }
}
