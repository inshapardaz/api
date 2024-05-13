using Inshapardaz.Domain.Adapters.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Adapters;

public class EmailSender : ISendEmail
{
    private readonly Settings _appSettings;
    private readonly ISmtpClient _smtpClient;

    public EmailSender(IOptions<Settings> appSettings, ISmtpClient smtpClient)
    {
        _appSettings = appSettings.Value;
        _smtpClient = smtpClient;
    }

    public void Send(string to, string subject, string html, string from = null)
    {
        // create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? _appSettings.Email.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        _smtpClient.Connect(_appSettings.Email.SmtpHost, _appSettings.Email.SmtpPort, _appSettings.Email.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        try
        {
            if (!string.IsNullOrWhiteSpace(_appSettings.Email.SmtpUser))
            {
                _smtpClient.Authenticate(_appSettings.Email.SmtpUser, _appSettings.Email.SmtpPass);
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
        email.From.Add(MailboxAddress.Parse(from ?? _appSettings.Email.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        await _smtpClient.ConnectAsync(_appSettings.Email.SmtpHost, _appSettings.Email.SmtpPort, _appSettings.Email.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);
        if (!string.IsNullOrWhiteSpace(_appSettings.Email.SmtpUser))
        {
            await _smtpClient.AuthenticateAsync(_appSettings.Email.SmtpUser, _appSettings.Email.SmtpPass, cancellationToken);
        }

        await _smtpClient.SendAsync(email, cancellationToken);
        await _smtpClient.DisconnectAsync(true, cancellationToken);
    }
}
