using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly BillingDataSource _dataSource;

        public BillingRepository(BillingDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<WaWabaUsageSummary>> GetAnalyticsAsync()
        {
            var result = await _dataSource.FetchAnalytics();
            return result.IsSuccess
                ? Result<WaWabaUsageSummary>.Success(ToSummary(result.Value))
                : Result<WaWabaUsageSummary>.Failure(result.Error);
        }

        public async Task<Result<List<WaWabaUsageSummary>>> GetUsageSummaryAsync(
            DateTime? start = null, DateTime? end = null, string wabaId = null)
        {
            var result = await _dataSource.FetchUsageSummary(start, end, wabaId);
            if (result.IsFailure)
                return Result<List<WaWabaUsageSummary>>.Failure(result.Error);

            return Result<List<WaWabaUsageSummary>>.Success(result.Value.Select(ToSummary).ToList());
        }

        public async Task<Result<bool>> SyncBillingAsync(string startTime, string endTime, string wabaId = null)
        {
            return await _dataSource.SyncBilling(startTime, endTime, wabaId);
        }

        private static Company ToCompany(Payloads.Companies.CompanyPayload p)
        {
            return new Company
            {
                Id = p.Id,
                Name = p.Name,
                LimitMarketing = p.LimitMarketing,
                LimitUtility = p.LimitUtility,
                LimitAuthentication = p.LimitAuthentication,
                LimitService = p.LimitService,
                UsageMarketing = p.UsageMarketing,
                UsageUtility = p.UsageUtility,
                UsageAuthentication = p.UsageAuthentication,
                UsageService = p.UsageService,
                MetaCost = p.MetaCost,
                CreatedAt = p.CreatedAt
            };
        }

        private static WaWabaUsageSummary ToSummary(Payloads.Billing.WaWabaUsageSummaryPayload p)
        {
            return new WaWabaUsageSummary
            {
                MonthPeriod = p.MonthPeriod,
                TotalVolume = p.TotalVolume,
                MarketingCost = p.MarketingCost,
                UtilityCost = p.UtilityCost,
                AuthCost = p.AuthCost,
                ServiceCost = p.ServiceCost,
                TotalCost = p.TotalCost
            };
        }
    }
}
