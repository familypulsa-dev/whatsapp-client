using System.Threading.Tasks;

namespace WaDesktop.Domain.Interfaces
{
    public interface IPhoneRegistrationUseCase
    {
        Task<string> CreatePhoneNumberAsync(string wabaId, string cc, string phoneNumber, string verifiedName);
        Task RequestVerificationCodeAsync(string phoneNumberId, string method);
        Task VerifyCodeAsync(string phoneNumberId, string code);
        Task RegisterPhoneAsync(string phoneNumberId, string pin, string confirmPin);
    }
}
