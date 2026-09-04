using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Companies;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur company.</summary>
    public class CompanyDataSource : BaseDataSource
    {
        public CompanyDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<CompanyPayload>>> Fetch()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Companies.Base}", null);
            return result.IsSuccess
                ? ParseList<CompanyPayload>(result.Value)
                : Result<List<CompanyPayload>>.Failure(result.Error);
        }

        public async Task<Result<CompanyPayload>> Create(Newtonsoft.Json.Linq.JObject payload)
        {
            var result = await SendAsync(HttpMethod.Post, $"{BaseUrl}{ApiRoutes.Companies.Base}", payload);
            return result.IsSuccess
                ? ParseData<CompanyPayload>(result.Value)
                : Result<CompanyPayload>.Failure(result.Error);
        }

        /// <param name="payload">JObject dinamis: name + waba_id (null eksplisit = lepas).</param>
        public async Task<Result<CompanyPayload>> Update(string id, Newtonsoft.Json.Linq.JObject payload)
        {
            var url = $"{BaseUrl}{ApiRoutes.Companies.Base}/{id}";
            var result = await SendAsync(HttpMethod.Put, url, payload);
            return result.IsSuccess
                ? ParseData<CompanyPayload>(result.Value)
                : Result<CompanyPayload>.Failure(result.Error);
        }

        public async Task<Result<bool>> Delete(string id)
        {
            var url = $"{BaseUrl}{ApiRoutes.Companies.Base}/{id}";
            var result = await SendAsync(HttpMethod.Delete, url, null);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }
    }
}
