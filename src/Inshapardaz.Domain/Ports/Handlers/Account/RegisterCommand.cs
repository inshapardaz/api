using Inshapardaz.Domain.Common;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class RegisterCommand : RequestBase
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string InvitationCode { get; set; }
        public bool AcceptTerms { get; set; }
    }

    public class RegisterCommandHandler : RequestHandlerAsync<RegisterCommand>

    {
        private readonly IAccountRepository _accountRepository;

        public RegisterCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override async Task<RegisterCommand> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
        {
            if (!command.AcceptTerms)
            {
                throw new BadRequestException("Terms must be accepted");
            }

            var account = await _accountRepository.GetAccountByInvitationCode(command.InvitationCode, cancellationToken);

            if (account == null || account.InvitationCodeExpiry < DateTime.UtcNow)
            {
                throw new BadRequestException("Invitation has expired");
            }

            account.Name = command.Name;
            account.PasswordHash = SecretHasher.GetStringHash(command.Password);
            account.Verified = DateTime.UtcNow;
            account.VerificationToken = RandomGenerator.GenerateRandomString();
            account.InvitationCode = null;
            account.InvitationCodeExpiry = null;
            account.AcceptTerms = true;

            await _accountRepository.UpdateAccount(account, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
