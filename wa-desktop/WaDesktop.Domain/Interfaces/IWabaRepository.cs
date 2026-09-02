using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    public interface IWabaRepository
    {
        Task<Result<List<Waba>>> GetAllAsync();
        Task<Result<bool>> UpdateCompanyAsync(string wabaId, string companyId);
        Task<Result<bool>> SyncFromMetaAsync();
    }
}
