namespace Inshapardaz.Domain.Adapters.Configuration
{
    public record Email
    {
        public string EmailFrom { get; init; }
        public string EmailFromName { get; init; }
        public string SmtpHost { get; init; }
        public int SmtpPort { get; init; }
        public string SmtpUser { get; init; }
        public string SmtpPass { get; init; }
        public bool SmtpTls { get; init; }
        public bool SmtpSsl { get; init; }
    }
}
