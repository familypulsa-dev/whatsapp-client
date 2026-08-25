using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>
    /// CRUD company. Fitur limit sudah dihapus dari backend — update hanya
    /// membawa name + waba_id. Assign WABA: backend otomatis me-unassign
    /// waba yang sama dari company lain (UnassignByCompany).
    /// wabaId null = lepas relasi.
    /// </summary>
    public interface ICompanyRepository
    {
        Task<Result<List<Company>>> GetAllAsync();
        Task<Result<Company>> CreateAsync(string name, string wabaId = null);
        Task<Result<Company>> UpdateAsync(string id, string name, string wabaId);
        Task<Result<bool>> DeleteAsync(string id);
    }
}
