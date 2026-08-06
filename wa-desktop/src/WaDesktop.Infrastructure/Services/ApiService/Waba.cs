using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public Task<List<Waba>> GetWabasAsync()
            => GetListAsync<Waba>(ApiRoutes.Waba.Base);

        public Task SyncWabasFromMetaAsync()
            => PostAsync<object>($"{ApiRoutes.Waba.Base}/sync", null);

        public Task UpdateWabaAsync(string wabaId, string companyId)
            => PutAsync($"{ApiRoutes.Waba.Base}/{wabaId}", new { company_id = companyId });

        public Task<List<WaWabaUsageSummary>> GetBillingSummaryAsync(DateTime? start = null, DateTime? end = null, string wabaId = null)
        {
            var parameters = new List<string>();
            if (start.HasValue) parameters.Add($"start={new DateTimeOffset(start.Value.ToUniversalTime()).ToUnixTimeSeconds()}");
            if (end.HasValue) parameters.Add($"end={new DateTimeOffset(end.Value.ToUniversalTime()).ToUnixTimeSeconds()}");
            if (!string.IsNullOrEmpty(wabaId)) parameters.Add($"waba_id={wabaId}");

            var url = ApiRoutes.Waba.Usage;
            if (parameters.Count > 0) url += "?" + string.Join("&", parameters);

            return GetListAsync<WaWabaUsageSummary>(url);
        }
    }
}