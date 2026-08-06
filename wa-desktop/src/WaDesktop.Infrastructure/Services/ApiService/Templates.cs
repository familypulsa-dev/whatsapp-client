using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public Task<List<Template>> GetTemplatesAsync(string search = null, string waba_id = null)
        {
            var parameters = new List<string>();
            if (!string.IsNullOrEmpty(waba_id)) parameters.Add($"waba_id={waba_id}");
            if (!string.IsNullOrEmpty(search)) parameters.Add($"search={search}");

            var url = ApiRoutes.Templates.Base;
            if (parameters.Count > 0) url += "?" + string.Join("&", parameters);
            return GetListAsync<Template>(url);
        }

        public Task SyncTemplatesAsync(string wabaId)
            => PostAsync($"{ApiRoutes.Templates.Base}/sync", new { waba_id = wabaId });

        public Task DeleteTemplateAsync(string id)
            => DeleteRequestAsync($"{ApiRoutes.Templates.Base}/{id}");
    }
}