namespace WaDesktop.Domain.Entities
{
    public class AppSetting
    {
        public string WabaToken { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string BusinessId { get; set; }
        public string VerifyToken { get; set; }
        public string WebhookUrl { get; set; }
        public bool MessageCleanupEnabled { get; set; }
        public int MessageRetentionDays { get; set; }
    }
}
