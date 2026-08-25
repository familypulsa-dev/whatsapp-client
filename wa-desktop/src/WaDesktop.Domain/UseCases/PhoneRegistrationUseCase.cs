using System;
using System.Threading.Tasks;
using WaDesktop.Domain.Entities;
using WaDesktop.Domain.Interfaces;

namespace WaDesktop.Domain.UseCases
{
    /// <summary>
    /// Orkestrasi registrasi nomor telepon Meta. Melempar Exception dengan pesan
    /// yang bisa ditampilkan langsung oleh PhoneRegistrationPresenter.
    /// </summary>
    public class PhoneRegistrationUseCase : IPhoneRegistrationUseCase
    {
        private readonly IPhoneNumberRepository _phones;

        public PhoneRegistrationUseCase(IPhoneNumberRepository phones)
        {
            _phones = phones;
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

            var result = await _phones.CreatePhoneNumberAsync(wabaId, req);
            if (result.IsFailure)
                throw new Exception(result.Error.Message);
            return result.Value.PhoneNumberId;
        }

        public async Task RequestVerificationCodeAsync(string phoneNumberId, string method)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("Phone Number ID belum tersedia.");

            var result = await _phones.RequestVerificationCodeAsync(phoneNumberId, new RequestCodeRequest
            {
                CodeMethod = method,
                Language = "en_US"
            });
            if (result.IsFailure)
                throw new Exception(result.Error.Message);
        }

        public async Task VerifyCodeAsync(string phoneNumberId, string code)
        {
            if (string.IsNullOrWhiteSpace(phoneNumberId))
                throw new ArgumentException("Phone Number ID belum tersedia.");

            code = code?.Trim().Replace("-", "").Replace(" ", "");
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Kode verifikasi wajib diisi.");

            var result = await _phones.VerifyCodeAsync(phoneNumberId, new VerifyCodeRequest { Code = code });
            if (result.IsFailure)
                throw new Exception(result.Error.Message);
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

            var result = await _phones.RegisterPhoneAsync(phoneNumberId, new RegisterPhoneRequest { Pin = pin });
            if (result.IsFailure)
                throw new Exception(result.Error.Message);
        }
    }
}
