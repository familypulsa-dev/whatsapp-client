using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly TemplateDataSource _dataSource;

        public TemplateRepository(TemplateDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<List<Template>>> GetAllAsync(string search = null, string wabaId = null)
        {
            var result = await _dataSource.Fetch(search, wabaId);
            if (result.IsFailure)
                return Result<List<Template>>.Failure(result.Error);

            return Result<List<Template>>.Success(result.Value.Select(ToEntity).ToList());
        }

        public Task<Result<bool>> SyncAsync(string wabaId)
            => _dataSource.Sync(wabaId);

        public Task<Result<bool>> DeleteAsync(string id)
            => _dataSource.Delete(id);

        private static Template ToEntity(Payloads.Templates.TemplatePayload p)
        {
            return new Template
            {
                Id = p.Id,
                WabaId = p.WabaId,
                Name = p.Name,
                Language = p.Language,
                Status = p.Status,
                Category = p.Category,
                MessageSendTtlSeconds = p.MessageSendTtlSeconds,
                ParameterFormat = p.ParameterFormat
            };
        }
    }
}
