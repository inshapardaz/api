using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Microsoft.Extensions.Logging;
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

        public int? Revoker { get; set; }
        public string Token { get; }

        public TokenResponse Response { get; set; }
    }

    public class RevokeTokenCommandHandler : RequestHandlerAsync<RevokeTokenCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGetIPAddress _ipAddressGetter;
        private readonly ILogger<RevokeTokenCommandHandler> _logger;

        public RevokeTokenCommandHandler(IAccountRepository accountRepository,
            IGetIPAddress ipAddressGetter, ILogger<RevokeTokenCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _ipAddressGetter = ipAddressGetter;
            _logger = logger;
        }

        public override async Task<RevokeTokenCommand> HandleAsync(RevokeTokenCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(command.Token))
            {
                _logger.LogInformation("Refresh token provided is null");
                throw new BadRequestException("Token is required");
            }
            
            if (command.Revoker  == null)
            {
                return await base.HandleAsync(command, cancellationToken);
            }

            var refreshToken = await _accountRepository.GetRefreshToken(command.Token, cancellationToken);
            var revoker = await _accountRepository.GetAccountById(command.Revoker.Value, cancellationToken);

            if (refreshToken != null)
            {
                if (refreshToken.AccountId != revoker.Id && !revoker.IsSuperAdmin)
                {
                    _logger.LogError("Unable to revoke token. Only admin and user themselves can revoke tokens");
                    throw new UnauthorizedException();
                }

                var ipAddress = _ipAddressGetter.GetIPAddressFromRequest();

                await _accountRepository.RevokeRefreshToken(refreshToken.Token, ipAddress, null, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Refresh token provided is found");
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
