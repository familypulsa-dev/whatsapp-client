using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    /// <summary>Mapping payload ↔ entity + aturan bisnis limit-opsional.</summary>
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CompanyDataSource _dataSource;

        public CompanyRepository(CompanyDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<List<Company>>> GetAllAsync()
        {
            var result = await _dataSource.Fetch();
            if (result.IsFailure)
                return Result<List<Company>>.Failure(result.Error);

            return Result<List<Company>>.Success(result.Value.Select(ToEntity).ToList());
        }

        public async Task<Result<Company>> CreateAsync(string name, string wabaId = null)
        {
            var payload = new Newtonsoft.Json.Linq.JObject { ["name"] = name };
            if (!string.IsNullOrEmpty(wabaId)) payload["waba_id"] = wabaId;

            var result = await _dataSource.Create(payload);
            return result.IsSuccess
                ? Result<Company>.Success(ToEntity(result.Value))
                : Result<Company>.Failure(result.Error);
        }

        public async Task<Result<Company>> UpdateAsync(string id, string name, string wabaId)
        {
            // waba_id selalu ikut (null eksplisit = lepas relasi).
            var payload = new Newtonsoft.Json.Linq.JObject
            {
                ["name"] = name,
                ["waba_id"] = wabaId == null ? Newtonsoft.Json.Linq.JValue.CreateNull() : new Newtonsoft.Json.Linq.JValue(wabaId)
            };

            var result = await _dataSource.Update(id, payload);
            return result.IsSuccess
                ? Result<Company>.Success(ToEntity(result.Value))
                : Result<Company>.Failure(result.Error);
        }

        public async Task<Result<bool>> DeleteAsync(string id)
        {
            return await _dataSource.Delete(id);
        }

        private static Company ToEntity(Payloads.Companies.CompanyPayload p)
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
                WabaId = p.WabaId,
                MetaCost = p.MetaCost,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
