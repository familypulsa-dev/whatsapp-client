using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>Data billing/analytics: ringkasan pemakaian (bentuk company) dan riwayat tagihan.</summary>
    public interface IBillingRepository
    {
        Task<Result<WaWabaUsageSummary>> GetAnalyticsAsync();
        Task<Result<List<WaWabaUsageSummary>>> GetUsageSummaryAsync(
            DateTime? start = null, DateTime? end = null, string wabaId = null);
    }
}
