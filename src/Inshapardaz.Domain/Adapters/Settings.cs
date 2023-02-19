using System;

namespace Inshapardaz.Domain.Adapters
{
    public class Settings
    {
        public string DefaultConnection { get; set; }
        public string CDNAddress { get; set; }
        public string BlobRoot { get; set; }
        public string FileStorageConnectionString { get; set; }

        public FileStoreTypes FileStoreType { get; set; }
        public string[] AllowedOrigins { 
            get 
            {
                return Allowed_Origins.Split(',', StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);
            }  
        }

        public string Allowed_Origins { get; set; }

        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public bool SmtpTls { get; set; }
        public bool SmtpSsl { get; set; }

        public string Secret { get; set; }
        public string FrontEndUrl { get; set; }

        public string RegisterPagePath { get; set; }
        public string ResetPasswordPagePath { get; set; }
        public int AccessTokenTTLInMinutes { get; internal set; } = 15;
        public double ResetTokenTTLInDays { get; internal set; } = 1;

        public int RefreshTokenTTLInDays { get; set; }
        public string S3ServiceUrl { get; set; }
        public string S3Accesskey { get; set; }
        public string S3AccessSecret { get; set; }
        public string S3BucketName { get; set; }
    }
}
