using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class RevokeTokenCommand : RequestBase
    {
        public RevokeTokenCommand(string token)
        {
            Token = token;
        }

        public AccountModel Revoker { get; set; }
        public string Token { get; }

        public TokenResponse Response { get; set; }
    }

    public class RevokeTokenCommandHandler : RequestHandlerAsync<RevokeTokenCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGetIPAddress _ipAddressGetter;

        public RevokeTokenCommandHandler(IAccountRepository accountRepository,
            IGetIPAddress ipAddressGetter)
        {
            _accountRepository = accountRepository;
            _ipAddressGetter = ipAddressGetter;
        }

        public override async Task<RevokeTokenCommand> HandleAsync(RevokeTokenCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(command.Token))
            {
                throw new BadRequestException("Token is required");
            }

            var refreshToken = await _accountRepository.GetRefreshToken(command.Token, cancellationToken);

            if (refreshToken == null)
            {
                throw new BadRequestException();
            }

            if (refreshToken.AccountId != command.Revoker.Id && !command.Revoker.IsSuperAdmin)
            {
                throw new UnauthorizedException();
            }

            var account = await _accountRepository.GetAccountById(refreshToken.AccountId, cancellationToken);
            var ipAddress = _ipAddressGetter.GetIPAddressFromRequest();

            await _accountRepository.RevokeRefreshToken(refreshToken.Token, ipAddress, null, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
