using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }

    public class EmailService : IEmailService
    {
        private readonly Settings _appSettings;

        public EmailService(Settings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, _appSettings.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            if (!string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
            {
                smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
            }
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
