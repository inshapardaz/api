using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class PasswordResetCommand : RequestBase
    {
        public PasswordResetCommand(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }

    public class PasswordResetCommandHandler : RequestHandlerAsync<PasswordResetCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly Settings _settings;
        private readonly IGenerateToken _tokenGenerator;
        private readonly IEmailService _emailService;

        public PasswordResetCommandHandler(IAccountRepository accountRepository,
            Settings settings,
            IGenerateToken tokenGenerator, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _settings = settings;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
        }

        public override async Task<PasswordResetCommand> HandleAsync(PasswordResetCommand command, CancellationToken cancellationToken = default)
        {
            var account = await _accountRepository.GetAccountByEmail(command.Email, cancellationToken);

            if (account == null)
            {
                return await base.HandleAsync(command, cancellationToken);
            }

            var resetToken = _tokenGenerator.GenerateResetToken();

            account.ResetToken = resetToken;
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(_settings.ResetTokenTTLInDays);

            await _accountRepository.UpdateAccount(account, cancellationToken);

            await _emailService.SendAsync(account.Email,
                    $"Reset your Password",
                    EmailTemplateProvider.GetResetPasswordEmail(account.Name, new Uri(new Uri(_settings.FrontEndUrl), _settings.ResetPasswordUrl).ToString()),
                    _settings.EmailFrom,
                    cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
