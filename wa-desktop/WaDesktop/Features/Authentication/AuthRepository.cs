using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDataSource _dataSource;

        public AuthRepository(AuthDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<AuthResult>> LoginAsync(string username, string password)
        {
            var result = await _dataSource.Login(username, password);
            if (result.IsFailure)
                return Result<AuthResult>.Failure(result.Error);

            // Guard: respons sukses tanpa token tetap dianggap gagal.
            if (string.IsNullOrEmpty(result.Value?.AccessToken))
                return Result<AuthResult>.Failure(Error.Validation("Login response did not contain an access token."));

            return Result<AuthResult>.Success(result.Value);
        }
    }
}
