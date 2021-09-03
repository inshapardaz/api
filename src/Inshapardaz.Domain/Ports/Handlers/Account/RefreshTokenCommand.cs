using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
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

        public RefreshTokenCommandHandler(IAccountRepository accountRepository,
            Settings settings,
            IGenerateToken tokenGenerator,
            IGetIPAddress ipAddressGetter)
        {
            _accountRepository = accountRepository;
            _settings = settings;
            _tokenGenerator = tokenGenerator;
            _ipAddressGetter = ipAddressGetter;
        }

        public override async Task<RefreshTokenCommand> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            var refreshToken = await _accountRepository.GetRefreshToken(command.Token, cancellationToken);
            var account = await _accountRepository.GetAccountById(refreshToken.AccountId, cancellationToken);
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
