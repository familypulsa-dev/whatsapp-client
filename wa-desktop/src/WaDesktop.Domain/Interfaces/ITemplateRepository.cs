using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    public interface ITemplateRepository
    {
        Task<Result<List<Template>>> GetAllAsync(string search = null, string wabaId = null);
        Task<Result<bool>> SyncAsync(string wabaId);
        Task<Result<bool>> DeleteAsync(string id);
    }
}
