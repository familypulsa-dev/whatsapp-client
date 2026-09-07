using System.Threading.Tasks;

namespace WaDesktop.Domain.Interfaces
{
    public interface IAuthTokenRefresher
    {
        Task<bool> TryRefreshAsync();
    }
}
