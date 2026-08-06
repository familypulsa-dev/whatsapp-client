using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Domain.UseCases
{
    public class PhoneRegistrationUseCase : IPhoneRegistrationUseCase
    {
        private readonly IApiClient _api;

        public PhoneRegistrationUseCase(IApiClient api)
        {
            _api = api;
        }

        public async Task<string> CreatePhoneNumberAsync(string wabaId, string cc, string phoneNumber, string verifiedName)
        {
            if (string.IsNullOrWhiteSpace(cc) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(verifiedName))
                throw new ArgumentException("Semua field (Country Code, Phone Number, Verified Name) wajib diisi.");

            var req = new CreatePhoneNumberRequest
            {
                Cc = cc.Trim(),
                PhoneNumber = phoneNumber.Trim(),
                VerifiedName = verifiedName.Trim(),
            };

            var resp = await _api.CreatePhoneNumberAsync(wabaId, req);
            return resp.PhoneNumberId;
        }

        public async Task RequestVerificationCodeAsync(string phoneNumberId, string method)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("Phone Number ID belum tersedia.");

            await _api.RequestVerificationCodeAsync(phoneNumberId, new RequestCodeRequest
            {
                CodeMethod = method,
                Language = "en_US"
            });
        }

        public async Task VerifyCodeAsync(string phoneNumberId, string code)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("Phone Number ID belum tersedia.");

            code = code?.Trim().Replace("-", "").Replace(" ", "");
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Kode verifikasi wajib diisi.");

            await _api.VerifyCodeAsync(phoneNumberId, new VerifyCodeRequest { Code = code });
        }

        public async Task RegisterPhoneAsync(string phoneNumberId, string pin, string confirmPin)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("Phone Number ID belum tersedia.");

            pin = pin?.Trim();
            if (string.IsNullOrWhiteSpace(pin) || pin.Length != 6 || !int.TryParse(pin, out _))
                throw new ArgumentException("PIN harus berupa 6 digit angka.");

            confirmPin = confirmPin?.Trim();
            if (pin != confirmPin)
                throw new ArgumentException("PIN dan konfirmasi PIN tidak cocok.");

            await _api.RegisterPhoneAsync(phoneNumberId, new RegisterPhoneRequest { Pin = pin });
        }
    }
}
