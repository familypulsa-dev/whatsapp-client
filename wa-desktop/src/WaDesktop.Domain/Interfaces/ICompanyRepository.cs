using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>
    /// CRUD company. Limit bersifat opsional: hanya dikirim ke backend
    /// bila diisi (update nama saja tidak akan menyentuh limit existing).
    /// </summary>
    public interface ICompanyRepository
    {
        Task<Result<List<Company>>> GetAllAsync();
        Task<Result<Company>> CreateAsync(string name);
        Task<Result<Company>> UpdateAsync(string id, string name,
            int? limitMarketing = null, int? limitUtility = null,
            int? limitAuth = null, int? limitService = null);
        Task<Result<bool>> DeleteAsync(string id);
    }
}
