using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDataSource _dataSource;

        public UserRepository(UserDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<List<User>>> GetAllAsync()
        {
            var result = await _dataSource.Fetch();
            if (result.IsFailure)
                return Result<List<User>>.Failure(result.Error);

            return Result<List<User>>.Success(result.Value.Select(ToEntity).ToList());
        }

        public async Task<Result<User>> CreateAsync(string username, string password, string name, string role, string companyId)
        {
            var result = await _dataSource.Create(username, password, name, role, companyId);
            return result.IsSuccess
                ? Result<User>.Success(ToEntity(result.Value))
                : Result<User>.Failure(result.Error);
        }

        public Task<Result<bool>> UpdateAsync(string id, string displayName, string role, string companyId, bool? isActive = null, bool? isSuspend = null)
            => _dataSource.Update(id, displayName, role, companyId, isActive, isSuspend);

        public Task<Result<bool>> DeactivateAsync(string id)
            => _dataSource.Deactivate(id);

        public Task<Result<bool>> ResetPasswordAsync(string id, string newPassword)
            => _dataSource.ResetPassword(id, newPassword);

        private static User ToEntity(Payloads.Users.UserPayload p)
        {
            return new User
            {
                Id = p.Id,
                Username = p.Username,
                Email = p.Email,
                DisplayName = p.Name,
                Role = p.Role,
                CompanyId = p.CompanyId,
                IsActive = p.IsActive,
                IsSuspend = p.IsSuspend
            };
        }
    }
}
