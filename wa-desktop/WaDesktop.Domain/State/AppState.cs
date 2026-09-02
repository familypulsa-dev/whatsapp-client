using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.State
{
    /// <summary>Single source of truth untuk state global aplikasi.</summary>
    public class AppState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public string DisplayName { get; set; }
        public string CompanyName { get; set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
        public bool IsSuperAdmin => Role == "super_admin";
        public string CompanyId { get; set; }

        public void SetSession(string accessToken, string refreshToken, string role, string displayName, string companyName, string companyId)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Role = role;
            DisplayName = displayName;
            CompanyName = companyName;
            CompanyId = companyId;
        }

        public void ClearSession()
        {
            AccessToken = null;
            RefreshToken = null;
            Role = null;
            DisplayName = null;
            CompanyName = null;
            CompanyId= null;
        }
    }
}
