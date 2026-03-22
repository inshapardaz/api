using Inshapardaz.Domain.Adapters.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Inshapardaz.Domain.Adapters;

public class EmailSender(IOptions<Settings> appSettings, ISmtpClient smtpClient) : ISendEmail
{
    private readonly Settings _appSettings = appSettings.Value;

    public void Send(string to, string subject, string html, string from = null)
    {
        // create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? _appSettings.Email.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        // send email
        smtpClient.Connect(_appSettings.Email.SmtpHost, _appSettings.Email.SmtpPort, _appSettings.Email.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        try
        {
            if (!string.IsNullOrWhiteSpace(_appSettings.Email.SmtpUser))
            {
                smtpClient.Authenticate(_appSettings.Email.SmtpUser, _appSettings.Email.SmtpPass);
            }
            smtpClient.Send(email);
        }
        finally
        {
            smtpClient.Disconnect(true);
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

        await smtpClient.ConnectAsync(_appSettings.Email.SmtpHost, _appSettings.Email.SmtpPort, _appSettings.Email.SmtpTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);
        if (!string.IsNullOrWhiteSpace(_appSettings.Email.SmtpUser))
        {
            await smtpClient.AuthenticateAsync(_appSettings.Email.SmtpUser, _appSettings.Email.SmtpPass, cancellationToken);
        }

        await smtpClient.SendAsync(email, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}
