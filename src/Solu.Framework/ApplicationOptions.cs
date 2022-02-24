namespace Solu.Framework
{
    public class ApplicationOptions
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string JwtSecret { get; set; } = null!;
        public int MaximumItemsInDashboard { get; set; } = 10;
        public string ReCaptchaSiteKey { get; set; }
        public string ReCaptchaSecretKey { get; set; }
        public string NotificationProfileId { get; set; }
        public string NotificationMessageId { get; set; }
        public string NotificationParameterName { get; set; }
        public long MaximumUploadSizeInBytes { get; set; }
    }
}
