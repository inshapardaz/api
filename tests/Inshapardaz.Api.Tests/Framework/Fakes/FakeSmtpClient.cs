using Inshapardaz.Api.Tests.Framework.Asserts;
using FluentAssertions;
using MailKit;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Framework.Fakes
{
    public class FakeSmtpClient : ISmtpClient
    {
        public class EmailMessage
        {
            public EmailMessage(MimeMessage message)
            {
                From = message.From.ToString();
                To = message.To.First().ToString();
                Subject = message.Subject;
                Body = message.BodyParts.OfType<TextPart>().FirstOrDefault().ToString();
            }

            public string From { get; private set; }
            public string To { get; private set; }
            public string Subject { get; private set; }
            public string Body { get; private set; }
        }

        internal void AssertNoEmailSent()
        {
            EmailsSent.Should().BeEmpty();
        }

        public List<EmailMessage> EmailsSent = new List<EmailMessage>();

        public SmtpCapabilities Capabilities => new SmtpCapabilities() { };

        internal EmailAssert AssertEmailSentTo(string adminEmail)
        {
            var sentEmail = EmailsSent.FirstOrDefault(e => e.To == adminEmail);
            sentEmail.Should().NotBeNull($"Email is not sent to ${adminEmail}");
            return new EmailAssert(sentEmail);
        }

        public string LocalDomain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public uint MaxSize => throw new NotImplementedException();

        public DeliveryStatusNotificationType DeliveryStatusNotificationType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object SyncRoot => throw new NotImplementedException();

        public SslProtocols SslProtocols { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CipherSuitesPolicy SslCipherSuitesPolicy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public X509CertificateCollection ClientCertificates { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CheckCertificateRevocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IPEndPoint LocalEndPoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IProxyClient ProxyClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public HashSet<string> AuthenticationMechanisms => throw new NotImplementedException();

        public bool IsAuthenticated { get; set; }

        public bool IsConnected { get; set; }

        public bool IsSecure => true;

        public bool IsEncrypted => true;

        public bool IsSigned => true;

        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SslProtocols SslProtocol => throw new NotImplementedException();

        public CipherAlgorithmType? SslCipherAlgorithm => throw new NotImplementedException();

        public int? SslCipherStrength => throw new NotImplementedException();

        public HashAlgorithmType? SslHashAlgorithm => throw new NotImplementedException();

        public int? SslHashStrength => throw new NotImplementedException();

        public ExchangeAlgorithmType? SslKeyExchangeAlgorithm => throw new NotImplementedException();

        public int? SslKeyExchangeStrength => throw new NotImplementedException();

        public TlsCipherSuite? SslCipherSuite => throw new NotImplementedException();

        public bool RequireTLS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<MessageSentEventArgs> MessageSent;

        public event EventHandler<ConnectedEventArgs> Connected;

        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        public void Authenticate(ICredentials credentials, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
        }

        public void Authenticate(Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
        }

        public void Authenticate(Encoding encoding, string userName, string password, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
        }

        public void Authenticate(string userName, string password, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
        }

        public void Authenticate(SaslMechanism mechanism, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
        }

        public Task AuthenticateAsync(ICredentials credentials, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(Encoding encoding, string userName, string password, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(SaslMechanism mechanism, CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public void Connect(string host, int port, bool useSsl, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
        }

        public void Connect(string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
        }

        public void Connect(Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
        }

        public void Connect(Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
        }

        public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
            return Task.CompletedTask;
        }

        public Task ConnectAsync(string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
            return Task.CompletedTask;
        }

        public Task ConnectAsync(Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
            return Task.CompletedTask;
        }

        public Task ConnectAsync(Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default)
        {
            IsConnected = true;
            return Task.CompletedTask;
        }

        public void Disconnect(bool quit, CancellationToken cancellationToken = default)
        {
            IsConnected = false;
        }

        public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
        {
            IsConnected = false;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public InternetAddressList Expand(string alias, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<InternetAddressList> ExpandAsync(string alias, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void NoOp(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task NoOpAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Send(MimeMessage message, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
        }

        public void Send(MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
        }

        public void Send(FormatOptions options, MimeMessage message, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
        }

        public void Send(FormatOptions options, MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
        }

        public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
            return Task.CompletedTask;
        }

        public Task SendAsync(MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
            return Task.CompletedTask;
        }

        public Task SendAsync(FormatOptions options, MimeMessage message, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
            return Task.CompletedTask;
        }

        public Task SendAsync(FormatOptions options, MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            EmailsSent.Add(new EmailMessage(message));
            return Task.CompletedTask;
        }

        public MailboxAddress Verify(string address, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MailboxAddress> VerifyAsync(string address, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        string IMailTransport.Send(MimeMessage message, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        Task<string> IMailTransport.SendAsync(MimeMessage message, CancellationToken cancellationToken, ITransferProgress progress)
        {
            EmailsSent.Add(new EmailMessage(message));
            return Task.FromResult(string.Empty);
        }

        string IMailTransport.Send(MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        Task<string> IMailTransport.SendAsync(MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        string IMailTransport.Send(FormatOptions options, MimeMessage message, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        Task<string> IMailTransport.SendAsync(FormatOptions options, MimeMessage message, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        string IMailTransport.Send(FormatOptions options, MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }

        Task<string> IMailTransport.SendAsync(FormatOptions options, MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken, ITransferProgress progress)
        {
            throw new NotImplementedException();
        }
    }
}
