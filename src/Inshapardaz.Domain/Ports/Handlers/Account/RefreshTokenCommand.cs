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
    public class RefreshTokenCommand : RequestBase
    {
        public RefreshTokenCommand(string token)
        {
            Token = token;
        }

        public string Token { get; }

        public TokenResponse Response { get; set; }
    }

    public class RefreshTokenCommandHandler : RequestHandlerAsync<RefreshTokenCommand>

    {
        private readonly IAccountRepository _accountRepository;
        private readonly Settings _settings;
        private readonly IGenerateToken _tokenGenerator;
        private readonly IGetIPAddress _ipAddressGetter;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(IAccountRepository accountRepository,
            Settings settings,
            IGenerateToken tokenGenerator,
            IGetIPAddress ipAddressGetter,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _accountRepository = accountRepository;
            _settings = settings;
            _tokenGenerator = tokenGenerator;
            _ipAddressGetter = ipAddressGetter;
            _logger = logger;
        }

        public override async Task<RefreshTokenCommand> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Refreshing token");

            if (command.Token == null)
            {
                _logger.LogInformation("Refresh token provided is null");
                throw new BadRequestException();
            }
            var refreshToken = await _accountRepository.GetRefreshToken(command.Token, cancellationToken);
            if (refreshToken == null)
            {
                _logger.LogInformation("Refresh token provided is invalid/not issued");
                throw new NotFoundException();
            }

            var account = await _accountRepository.GetAccountById(refreshToken.AccountId, cancellationToken);
            if (account == null)
            {
                _logger.LogInformation("Account related to Refresh token not found");
                throw new NotFoundException();
            }

            var ipAddress = _ipAddressGetter.GetIPAddressFromRequest();

            var newRefreshToken = _tokenGenerator.GenerateRefreshToken(ipAddress);

            await _accountRepository.RevokeRefreshToken(refreshToken.Token, ipAddress, newRefreshToken.Token, cancellationToken);

            await _accountRepository.RemoveOldRefreshTokens(account, _settings.RefreshTokenTTLInDays, cancellationToken);

            var jwtToken = _tokenGenerator.GenerateJwtToken(account);

            command.Response = new TokenResponse
            {
                Account = account,
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
