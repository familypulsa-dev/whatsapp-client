using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Infrastructure.Constants;
using WaDesktop.Infrastructure.Payloads.PhoneNumbers;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>Akses HTTP mentah untuk fitur phone number.</summary>
    public class PhoneNumberDataSource : BaseDataSource
    {
        public PhoneNumberDataSource(HttpClient http, string baseUrl) : base(http, baseUrl)
        {
        }

        public async Task<Result<List<PhoneNumberNodePayload>>> FetchNodes()
        {
            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}", null);
            return result.IsSuccess
                ? ParseList<PhoneNumberNodePayload>(result.Value)
                : Result<List<PhoneNumberNodePayload>>.Failure(result.Error);
        }

        public async Task<Result<List<PhoneNumberDetailPayload>>> FetchAll(string wabaId = null)
        {
            var url = ApiRoutes.PhoneNumbers.Base;
            if (!string.IsNullOrEmpty(wabaId)) url += $"?waba_id={wabaId}";

            var result = await SendAsync(HttpMethod.Get, $"{BaseUrl}{url}", null);
            return result.IsSuccess
                ? ParseList<PhoneNumberDetailPayload>(result.Value)
                : Result<List<PhoneNumberDetailPayload>>.Failure(result.Error);
        }

        public async Task<Result<PhoneNumberDetailPayload>> FetchDetail(string phoneNumberId)
        {
            var url = $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}";
            var result = await SendAsync(HttpMethod.Get, url, null);
            return result.IsSuccess
                ? ParseData<PhoneNumberDetailPayload>(result.Value)
                : Result<PhoneNumberDetailPayload>.Failure(result.Error);
        }

        public Task<Result<byte[]>> FetchProfilePicture(string url)
        {
            var absoluteUrl = url.StartsWith("http") ? url : $"{BaseUrl}{url}";
            return GetBytesAsync(absoluteUrl);
        }

        public async Task<Result<SavePhonePayload>> SaveDetail(string phoneNumberId, string displayName,
            string description, string email, string about, string address, string vertical, List<string> websites)
        {
            if (websites != null && websites.Count > 2) websites = websites.GetRange(0, 2);
            var payload = new { display_name = displayName, description, email, about, address, vertical, websites };

            var url = $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}";
            var result = await SendAsync(HttpMethod.Put, url, payload);
            return result.IsSuccess
                ? ParseData<SavePhonePayload>(result.Value)
                : Result<SavePhonePayload>.Failure(result.Error);
        }

        public async Task<Result<PhoneNumberDetailPayload>> SyncProfile(string phoneNumberId)
        {
            var url = $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/sync-profile";
            var result = await SendAsync(HttpMethod.Post, url, null);
            return result.IsSuccess
                ? ParseData<PhoneNumberDetailPayload>(result.Value)
                : Result<PhoneNumberDetailPayload>.Failure(result.Error);
        }

        public async Task<Result<bool>> SyncFromMeta(string wabaId)
        {
            var url = $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/sync";
            var result = await SendAsync(HttpMethod.Post, url, new { waba_id = wabaId });
            return result.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Error);
        }

        public async Task<Result<PhoneNumberDetailPayload>> UploadPicture(string phoneNumberId, string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(bytes), "file", fileName);

            var url = $"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/profile-picture";
            var result = await SendContentAsync(HttpMethod.Post, url, form);
            return result.IsSuccess
                ? ParseData<PhoneNumberDetailPayload>(result.Value)
                : Result<PhoneNumberDetailPayload>.Failure(result.Error);
        }

        // ── Phone Number Registration Flow ──

        public async Task<Result<CreatePhoneNumberResponse>> CreatePhoneNumber(string wabaId, CreatePhoneNumberRequest request)
        {
            var url = $"{BaseUrl}{ApiRoutes.Wabas.Base}/{wabaId}/phone-numbers";
            var result = await SendAsync(HttpMethod.Post, url, request);
            return result.IsSuccess
                ? ParseData<CreatePhoneNumberResponse>(result.Value)
                : Result<CreatePhoneNumberResponse>.Failure(result.Error);
        }

        public Task<Result<string>> RequestVerificationCode(string phoneNumberId, RequestCodeRequest request)
            => PostPlain($"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/request-code", request);

        public Task<Result<string>> VerifyCode(string phoneNumberId, VerifyCodeRequest request)
            => PostPlain($"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/verify-code", request);

        public Task<Result<string>> RegisterPhone(string phoneNumberId, RegisterPhoneRequest request)
            => PostPlain($"{BaseUrl}{ApiRoutes.PhoneNumbers.Base}/{phoneNumberId}/register", request);

        private async Task<Result<string>> PostPlain(string url, object body)
        {
            var result = await SendAsync(HttpMethod.Post, url, body);
            return result.IsSuccess
                ? Result<string>.Success(result.Value)
                : Result<string>.Failure(result.Error);
        }

        // ── Webhook Configuration ──

        public async Task<Result<WebhookConfig>> FetchWebhook(string phoneNumberId)
        {
            var url = $"{BaseUrl}{string.Format(ApiRoutes.PhoneNumbers.Webhook, phoneNumberId)}";
            var result = await SendAsync(HttpMethod.Get, url, null);
            return result.IsSuccess
                ? ParseData<WebhookConfig>(result.Value)
                : Result<WebhookConfig>.Failure(result.Error);
        }

        public async Task<Result<string>> SetWebhook(string phoneNumberId, string webhookUrl)
        {
            var url = $"{BaseUrl}{string.Format(ApiRoutes.PhoneNumbers.Webhook, phoneNumberId)}";
            var payload = new { webhook_url = webhookUrl };
            var result = await SendAsync(HttpMethod.Put, url, payload);
            return result.IsSuccess
                ? Result<string>.Success(result.Value)
                : Result<string>.Failure(result.Error);
        }
    }
}
