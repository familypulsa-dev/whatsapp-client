using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Templates;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur template.</summary>
    public class TemplateDataSource : BaseDataSource
    {
        public TemplateDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<TemplatePayload>>> Fetch(string search = null, string wabaId = null)
        {
            var parameters = new List<string>();
            if (!string.IsNullOrEmpty(wabaId)) parameters.Add($"waba_id={wabaId}");
            if (!string.IsNullOrEmpty(search)) parameters.Add($"search={Uri.EscapeDataString(search)}");

            var url = ApiRoutes.Templates.Base;
            if (parameters.Count > 0) url += "?" + string.Join("&", parameters);

            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{url}", null);
            return result.IsSuccess
                ? ParseList<TemplatePayload>(result.Value)
                : Result<List<TemplatePayload>>.Failure(result.Error);
        }

        public async Task<Result<bool>> Sync(string wabaId)
        {
            var url = $"{BaseUrl}{ApiRoutes.Templates.Base}/sync";
            var result = await SendAsync(HttpMethod.Post, url, new { waba_id = wabaId });
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<bool>> Delete(string id)
        {
            var url = $"{BaseUrl}{ApiRoutes.Templates.Base}/{id}";
            var result = await SendAsync(HttpMethod.Delete, url, null);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }
    }
}
