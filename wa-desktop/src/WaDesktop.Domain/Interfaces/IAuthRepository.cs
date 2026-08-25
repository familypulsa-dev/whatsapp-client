using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>Autentikasi pengguna. Refresh ditangani AuthDelegatingHandler.</summary>
    public interface IAuthRepository
    {
        Task<Result<AuthResult>> LoginAsync(string username, string password);
    }
}
