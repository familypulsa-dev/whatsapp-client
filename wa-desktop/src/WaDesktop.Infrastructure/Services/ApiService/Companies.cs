using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public Task<List<Company>> GetCompaniesAsync()
            => GetListAsync<Company>(ApiRoutes.Companies.Base);

        public Task<Company> CreateCompanyAsync(string name)
            => PostAsync<object, Company>(ApiRoutes.Companies.Base, new { name });

        public Task<Company> GetBillingAnalyticsAsync()
            => GetAsync<Company>(ApiRoutes.Analytics.Billing);

        public Task<Company> UpdateCompanyAsync(string id, string name, int? limitMarketing = null, int? limitUtility = null, int? limitAuth = null, int? limitService = null)
        {
            // Limit hanya dikirim jika diisi — update nama saja tidak akan menghapus limit existing.
            var payload = new Newtonsoft.Json.Linq.JObject { ["name"] = name };
            if (limitMarketing.HasValue) payload["limit_marketing"] = limitMarketing.Value;
            if (limitUtility.HasValue) payload["limit_utility"] = limitUtility.Value;
            if (limitAuth.HasValue) payload["limit_authentication"] = limitAuth.Value;
            if (limitService.HasValue) payload["limit_service"] = limitService.Value;

            return PutAsync<Newtonsoft.Json.Linq.JObject, Company>($"{ApiRoutes.Companies.Base}/{id}", payload);
        }

        public Task DeleteCompanyAsync(string id)
            => DeleteRequestAsync($"{ApiRoutes.Companies.Base}/{id}");
    }
}