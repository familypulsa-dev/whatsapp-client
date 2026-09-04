using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class WabaRepository : IWabaRepository
    {
        private readonly WabaDataSource _dataSource;

        public WabaRepository(WabaDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<List<Waba>>> GetAllAsync()
        {
            var result = await _dataSource.Fetch();
            if (result.IsFailure)
                return Result<List<Waba>>.Failure(result.Error);

            return Result<List<Waba>>.Success(result.Value.Select(ToEntity).ToList());
        }

        public Task<Result<bool>> UpdateCompanyAsync(string wabaId, string companyId)
            => _dataSource.UpdateCompany(wabaId, companyId);

        public Task<Result<bool>> SyncFromMetaAsync()
            => _dataSource.SyncFromMeta();

        private static Waba ToEntity(Payloads.Wabas.WabaPayload p)
        {
            return new Waba
            {
                WabaId = p.WabaId,
                CompanyId = p.CompanyId,
                CompanyName = p.CompanyName,
                Name = p.Name,
                Status = p.Status,
                MessagingLimit = p.MessagingLimit,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                LastSyncPricing = p.LastSyncPricing
            };
        }
    }
}
