namespace Inshapardaz.Domain.Adapters
{
    public class Settings
    {
        public string DefaultConnection { get; set; }
        public string CDNAddress { get; set; }
        public string BlobRoot { get; set; }
        public string FileStorageConnectionString { get; set; }

        public FileStoreTypes FileStoreType { get; set; }
        public string[] AllowedOrigins { get; set; }

        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public bool SmtpTls { get; set; }

        public string Secret { get; set; }
        public string FrontEndUrl { get; set; }
        public string ResetPasswordUrl { get; set; }
        public int AccessTokenTTLInMinutes { get; internal set; } = 15;
        public double ResetTokenTTLInDays { get; internal set; } = 1;

        public int RefreshTokenTTLInDays { get; set; }
    }
}
