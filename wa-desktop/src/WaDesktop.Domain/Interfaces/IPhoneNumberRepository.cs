using System.Collections.Generic;
using System.Threading.Tasks;
using WaDesktop.Domain.Common;
using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    /// <summary>
    /// Operasi phone number (profil & sinkronisasi). Method registrasi Meta
    /// (create/request-code/verify-code/register) tetap dijalankan lewat
    /// IPhoneRegistrationUseCase dan tidak masuk kontrak ini.
    /// </summary>
    public interface IPhoneNumberRepository
    {
        Task<Result<List<PhoneNumberNode>>> GetNodesAsync();
        Task<Result<List<PhoneNumberDetail>>> GetAllAsync(string wabaId = null);
        Task<Result<PhoneNumberDetail>> GetDetailAsync(string phoneNumberId);
        /// <summary>Bisa URL absolut (Meta CDN) atau relatif. 404 → Success(null).</summary>
        Task<Result<byte[]>> GetProfilePictureAsync(string url);
        Task<Result<SavePhoneResult>> SaveDetailAsync(string phoneNumberId, string displayName,
            string description, string email, string about, string address, string vertical, List<string> websites);
        Task<Result<PhoneNumberDetail>> SyncProfileAsync(string phoneNumberId);
        Task<Result<bool>> SyncFromMetaAsync(string wabaId);
        Task<Result<PhoneNumberDetail>> UploadPictureAsync(string phoneNumberId, string filePath);
    }
}
