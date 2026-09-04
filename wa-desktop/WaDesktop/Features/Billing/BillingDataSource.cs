using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.Billing;
using WaDesktop.Infrastructure.Payloads.Companies;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur billing/analytics.</summary>
    public class BillingDataSource : BaseDataSource
    {
        public BillingDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        /// <summary>Endpoint analytics mengembalikan data berbentuk company.</summary>
        public async Task<Result<WaWabaUsageSummaryPayload>> FetchAnalytics()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.Analytics.Billing}", null);
            return result.IsSuccess
                ? ParseData<WaWabaUsageSummaryPayload>(result.Value)
                : Result<WaWabaUsageSummaryPayload>.Failure(result.Error);
        }

        public async Task<Result<List<WaWabaUsageSummaryPayload>>> FetchUsageSummary(
            DateTime? start = null, DateTime? end = null, string wabaId = null)
        {
            var parameters = new List<string>();
            if (start.HasValue)
                parameters.Add($"start={new DateTimeOffset(start.Value.ToUniversalTime()).ToUnixTimeSeconds()}");
            if (end.HasValue)
                parameters.Add($"end={new DateTimeOffset(end.Value.ToUniversalTime()).ToUnixTimeSeconds()}");
            if (!string.IsNullOrEmpty(wabaId))
                parameters.Add($"waba_id={wabaId}");

            var url = ApiRoutes.Waba.Usage;
            if (parameters.Count > 0) url += "?" + string.Join("&", parameters);

            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{url}", null);
            return result.IsSuccess
                ? ParseList<WaWabaUsageSummaryPayload>(result.Value)
                : Result<List<WaWabaUsageSummaryPayload>>.Failure(result.Error);
        }

        public async Task<Result<bool>> SyncBilling(string startTime, string endTime, string wabaId = null)
        {
            var body = new
            {
                start_time = startTime,
                end_time = endTime,
                waba_id = string.IsNullOrEmpty(wabaId) ? null : wabaId
            };
            var result = await SendAsync(HttpMethod.Post, $"{BaseUrl}{ApiRoutes.Waba.SyncBilling}", body);
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }
    }
}
