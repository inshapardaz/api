using Inshapardaz.Domain.Adapters;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Inshapardaz.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly Settings _appSettings;
        private readonly ISmtpClient _smtpClient;

        public EmailService(Settings appSettings, ISmtpClient smtpClient)
        {
            _appSettings = appSettings;
            _smtpClient = smtpClient;
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
            _smtpClient.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, _appSettings.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            try
            {
                if (!string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
                {
                    _smtpClient.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
                }
                _smtpClient.Send(email);
            }

            finally
            {
                _smtpClient.Disconnect(true);
            }
        }

        public async Task SendAsync(string to, string subject, string html, string from = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            await _smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, _appSettings.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);
            if (!string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
            {
                await _smtpClient.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass, cancellationToken);
            }

            await _smtpClient.SendAsync(email, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }
}
