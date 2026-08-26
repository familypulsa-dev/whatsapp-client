using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;
using WaDesktop.Infrastructure.Data.Remote.DataSources;

namespace WaDesktop.Infrastructure.Data.Repositories
{
    public class PhoneNumberRepository : IPhoneNumberRepository
    {
        private readonly PhoneNumberDataSource _dataSource;

        public PhoneNumberRepository(PhoneNumberDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Result<List<PhoneNumberNode>>> GetNodesAsync()
        {
            var result = await _dataSource.FetchNodes();
            if (result.IsFailure)
                return Result<List<PhoneNumberNode>>.Failure(result.Error);

            return Result<List<PhoneNumberNode>>.Success(result.Value.Select(p => new PhoneNumberNode
            {
                PhoneNumberId = p.PhoneNumberId,
                DisplayName = p.DisplayName,
                DisplayPhoneNumber = p.DisplayPhoneNumber
            }).ToList());
        }

        public async Task<Result<List<PhoneNumberDetail>>> GetAllAsync(string wabaId = null)
        {
            var result = await _dataSource.FetchAll(wabaId);
            if (result.IsFailure)
                return Result<List<PhoneNumberDetail>>.Failure(result.Error);

            return Result<List<PhoneNumberDetail>>.Success(result.Value.Select(ToEntity).ToList());
        }

        public async Task<Result<PhoneNumberDetail>> GetDetailAsync(string phoneNumberId)
        {
            var result = await _dataSource.FetchDetail(phoneNumberId);
            return result.IsSuccess
                ? Result<PhoneNumberDetail>.Success(ToEntity(result.Value))
                : Result<PhoneNumberDetail>.Failure(result.Error);
        }

        public Task<Result<byte[]>> GetProfilePictureAsync(string url)
            => _dataSource.FetchProfilePicture(url);

        public async Task<Result<SavePhoneResult>> SaveDetailAsync(string phoneNumberId, string displayName,
            string description, string email, string about, string address, string vertical, List<string> websites)
        {
            var result = await _dataSource.SaveDetail(phoneNumberId, displayName,
                description, email, about, address, vertical, websites);
            if (result.IsFailure)
                return Result<SavePhoneResult>.Failure(result.Error);

            return Result<SavePhoneResult>.Success(new SavePhoneResult
            {
                Detail = ToEntity(result.Value.Detail),
                Warnings = result.Value.Warnings
            });
        }

        public async Task<Result<PhoneNumberDetail>> SyncProfileAsync(string phoneNumberId)
        {
            var result = await _dataSource.SyncProfile(phoneNumberId);
            return result.IsSuccess
                ? Result<PhoneNumberDetail>.Success(ToEntity(result.Value))
                : Result<PhoneNumberDetail>.Failure(result.Error);
        }

        public Task<Result<bool>> SyncFromMetaAsync(string wabaId)
            => _dataSource.SyncFromMeta(wabaId);

        public async Task<Result<PhoneNumberDetail>> UploadPictureAsync(string phoneNumberId, string filePath)
        {
            var result = await _dataSource.UploadPicture(phoneNumberId, filePath);
            return result.IsSuccess
                ? Result<PhoneNumberDetail>.Success(ToEntity(result.Value))
                : Result<PhoneNumberDetail>.Failure(result.Error);
        }

        // ── Phone Number Registration Flow ──

        public async Task<Result<CreatePhoneNumberResponse>> CreatePhoneNumberAsync(string wabaId, CreatePhoneNumberRequest request)
        {
            var result = await _dataSource.CreatePhoneNumber(wabaId, request);
            return result.IsSuccess
                ? Result<CreatePhoneNumberResponse>.Success(result.Value)
                : Result<CreatePhoneNumberResponse>.Failure(result.Error);
        }

        public async Task<Result> RequestVerificationCodeAsync(string phoneNumberId, RequestCodeRequest request)
        {
            var result = await _dataSource.RequestVerificationCode(phoneNumberId, request);
            return result.IsSuccess
                ? Result.Success()
                : Result.Failure(result.Error);
        }

        public async Task<Result> VerifyCodeAsync(string phoneNumberId, VerifyCodeRequest request)
        {
            var result = await _dataSource.VerifyCode(phoneNumberId, request);
            return result.IsSuccess
                ? Result.Success()
                : Result.Failure(result.Error);
        }

        public async Task<Result> RegisterPhoneAsync(string phoneNumberId, RegisterPhoneRequest request)
        {
            var result = await _dataSource.RegisterPhone(phoneNumberId, request);
            return result.IsSuccess
                ? Result.Success()
                : Result.Failure(result.Error);
        }

        // ── Webhook Configuration ──

        public async Task<Result<WebhookConfig>> GetWebhookAsync(string phoneNumberId)
        {
            var result = await _dataSource.FetchWebhook(phoneNumberId);
            if (result.IsFailure)
                return Result<WebhookConfig>.Failure(result.Error);

            var p = result.Value;
            return Result<WebhookConfig>.Success(new WebhookConfig
            {
                PhoneNumber = p.PhoneNumber,
                WhatsAppBusinessAccount = p.WhatsAppBusinessAccount,
                Application = p.Application
            });
        }

        public async Task<Result> SetWebhookAsync(string phoneNumberId, string webhookUrl)
        {
            var result = await _dataSource.SetWebhook(phoneNumberId, webhookUrl);
            return result.IsSuccess
                ? Result.Success()
                : Result.Failure(result.Error);
        }

        private static PhoneNumberDetail ToEntity(Payloads.PhoneNumbers.PhoneNumberDetailPayload p)
        {
            if (p == null) return null;
            return new PhoneNumberDetail
            {
                PhoneNumberId = p.PhoneNumberId,
                WabaId = p.WabaId,
                DisplayName = p.DisplayName,
                DisplayPhone = p.DisplayPhone,
                QualityRating = p.QualityRating,
                NameStatus = p.NameStatus,
                CodeVerificationStatus = p.CodeVerificationStatus,
                MetaStatus = p.MetaStatus,
                PinEnabled = p.PinEnabled,
                Description = p.Description,
                Email = p.Email,
                About = p.About,
                Address = p.Address,
                Vertical = p.Vertical,
                Websites = p.Websites,
                ProfilePictureUrl = p.ProfilePictureUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
