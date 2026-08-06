using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Constants;

namespace WaDesktop.Infrastructure.Services
{
    public partial class ApiClient
    {
        public async Task<List<PhoneNumberNode>> GetPhoneNumbersAsync()
        {
            var items = await GetListAsync<PhoneNumberDto>(ApiRoutes.PhoneNumbers.Base);
            return items.Select(dto => new PhoneNumberNode
            {
                PhoneNumberId = dto.PhoneNumberId,
                DisplayName = dto.DisplayName,
                DisplayPhoneNumber = dto.DisplayPhone,
            }).ToList();
        }

        private class PhoneNumberDto
        {
            [JsonProperty("phone_number_id")] public string PhoneNumberId { get; set; }
            [JsonProperty("display_phone_number")] public string DisplayPhone { get; set; }
            [JsonProperty("verified_name")] public string DisplayName { get; set; }
        }

        public async Task<List<PhoneNumberDetail>> GetPhoneNumberListAsync(string wabaId = null)
        {
            var url = ApiRoutes.PhoneNumbers.Base;
            if (!string.IsNullOrEmpty(wabaId)) url += $"?waba_id={wabaId}";
            return await GetListAsync<PhoneNumberDetail>(url);
        }

        public Task<PhoneNumberDetail> GetPhoneDetailAsync(string phoneNumberId)
            => GetAsync<PhoneNumberDetail>($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}");

        public async Task<byte[]> GetPhoneProfilePictureAsync(string url)
        {
            var absoluteUrl = url.StartsWith("http") ? url : $"{_baseUrl}{url}";
            var res = await SendWithRefreshAsync(() => _http.GetAsync(absoluteUrl));
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            await EnsureSuccessAsync(res);
            return await res.Content.ReadAsByteArrayAsync();
        }

        public Task<SavePhoneResult> SavePhoneDetailAsync(string phoneNumberId, string displayName, string description, string email, string about, string address, string vertical, List<string> websites)
        {
            if (websites != null && websites.Count > 2) websites = websites.GetRange(0, 2);
            var payload = new { display_name = displayName, description, email, about, address, vertical, websites };
            return PutAsync<object, SavePhoneResult>($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}", payload);
        }

        public Task<PhoneNumberDetail> SyncPhoneProfileAsync(string phoneNumberId)
            => PostAsync<object, PhoneNumberDetail>($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/sync-profile", null);

        public Task SyncPhoneNumbersFromMetaAsync(string wabaId)
            => PostAsync($"{ApiRoutes.PhoneNumbers.Base}/sync", new { waba_id = wabaId });

        public Task<CreatePhoneNumberResponse> CreatePhoneNumberAsync(string wabaId, CreatePhoneNumberRequest req)
            => PostAsync<CreatePhoneNumberRequest, CreatePhoneNumberResponse>($"{ApiRoutes.Wabas.Base}/{wabaId}/phone-numbers", req);

        public Task RequestVerificationCodeAsync(string phoneNumberId, RequestCodeRequest req)
            => PostAsync($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/request-code", req);

        public Task VerifyCodeAsync(string phoneNumberId, VerifyCodeRequest req)
            => PostAsync($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/verify-code", req);

        public Task RegisterPhoneAsync(string phoneNumberId, RegisterPhoneRequest req)
            => PostAsync($"{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/register", req);

        public async Task<PhoneNumberDetail> UploadPhonePictureAsync(string phoneNumberId, string filePath)
        {
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);
            
            Func<Task<HttpResponseMessage>> sendFunc = () =>
            {
                var form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(bytes);
                form.Add(fileContent, "file", fileName);
                return _http.PostAsync($"{_baseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/profile-picture", form);
            };

            var res = await SendWithRefreshAsync(sendFunc);
            await EnsureSuccessAsync(res);
                
            var json = await res.Content.ReadAsStringAsync();
            return UnwrapData<PhoneNumberDetail>(json);
        }
    }
}