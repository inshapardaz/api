using System.Net;
using System.Net.Mail;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }

    public class EmailService : IEmailService
    {
        private readonly Settings _appSettings;

        public EmailService(Settings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Send(string to, string subject, string html)
        {
            var fromAddress = new MailAddress(_appSettings.EmailFrom, _appSettings.EmailFromName);
            var toAddress = new MailAddress(to);

            using var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = _appSettings.SmtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_appSettings.SmtpUser, _appSettings.SmtpPass)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
