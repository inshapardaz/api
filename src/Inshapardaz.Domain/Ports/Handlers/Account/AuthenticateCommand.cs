using BC = BCrypt.Net.BCrypt;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class AuthenticateCommand : RequestBase
    {
        public AuthenticateCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; private set; }
        public string Password { get; private set; }

        public TokenResponse Response { get; set; }
    }

    public class AuthenticateQueryHandler : RequestHandlerAsync<AuthenticateCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenerateToken _tokenGenerator;
        private readonly IGetIPAddress _ipAddressGetter;
        private readonly Settings _settings;

        public AuthenticateQueryHandler(IAccountRepository accountRepository, IGenerateToken tokenGenerator, Settings settings, IGetIPAddress ipAddressGetter)
        {
            _accountRepository = accountRepository;
            _tokenGenerator = tokenGenerator;
            _settings = settings;
            _ipAddressGetter = ipAddressGetter;
        }

        public override async Task<AuthenticateCommand> HandleAsync(AuthenticateCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var account = await _accountRepository.GetAccountByEmail(command.Email, cancellationToken);

            if (account == null || !account.IsVerified || !BC.Verify(command.Password, account.PasswordHash))
            {
                throw new UnauthorizedException();
            }

            var jwtToken = _tokenGenerator.GenerateJwtToken(account);
            var refreshToken = _tokenGenerator.GenerateRefreshToken(_ipAddressGetter.GetIPAddressFromRequest());

            await _accountRepository.AddRefreshToken(refreshToken, account.Id, cancellationToken);
            await _accountRepository.RemoveOldRefreshTokens(account, _settings.RefreshTokenTTLInDays, cancellationToken);

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
