using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Result<List<User>>> GetAllAsync();
        Task<Result<User>> CreateAsync(string username, string password, string name, string role, string companyId);
        /// <param name="isActive">Hanya dikirim ke backend bila diisi.</param>
        /// <param name="isSuspend">Hanya dikirim ke backend bila diisi.</param>
        Task<Result<bool>> UpdateAsync(string id, string displayName, string role, string companyId, bool? isActive = null, bool? isSuspend = null);
        Task<Result<bool>> DeactivateAsync(string id);
        Task<Result<bool>> ResetPasswordAsync(string id, string newPassword);
    }
}
