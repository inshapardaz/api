using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Microsoft.Extensions.Options;
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
        private readonly ISendEmail _emailService;

        public PasswordResetCommandHandler(IAccountRepository accountRepository,
            IOptions<Settings> settings,
            IGenerateToken tokenGenerator, ISendEmail emailService)
        {
            _accountRepository = accountRepository;
            _settings = settings.Value;
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
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(_settings.Security.ResetTokenTTLInDays);

            await _accountRepository.UpdateAccount(account, cancellationToken);

            var resetLink = new Uri(new Uri(_settings.FrontEndUrl), _settings.Security.ResetPasswordPagePath + resetToken).ToString();
            await _emailService.SendAsync(account.Email,
                    $"Reset your Password",
                    EmailTemplateProvider.GetResetPasswordEmail(account.Name,resetLink),
                    _settings.Email.EmailFrom,
                    cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
