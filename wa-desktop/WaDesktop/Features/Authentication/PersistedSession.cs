namespace WaDesktop.Domain.Entities
{
    public class PersistedSession
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public string DisplayName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
    }
}
