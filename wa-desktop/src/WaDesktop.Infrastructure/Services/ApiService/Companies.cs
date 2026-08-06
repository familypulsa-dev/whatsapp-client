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
            var payload = new { name, limit_marketing = limitMarketing, limit_utility = limitUtility, limit_authentication = limitAuth, limit_service = limitService };
            return PutAsync<object, Company>($"{ApiRoutes.Companies.Base}/{id}", payload);
        }

        public Task DeleteCompanyAsync(string id)
            => DeleteRequestAsync($"{ApiRoutes.Companies.Base}/{id}");
    }
}