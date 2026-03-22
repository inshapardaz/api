using BC = BCrypt.Net.BCrypt;
using Paramore.Brighter;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class AuthenticateCommand(string email, string password) : RequestBase
{
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;

    public TokenResponse Response { get; set; }
}

public class AuthenticateQueryHandler(
    IAccountRepository accountRepository,
    IGenerateToken tokenGenerator,
    IOptions<Settings> settings,
    IGetIPAddress ipAddressGetter)
    : RequestHandlerAsync<AuthenticateCommand>
{
    private readonly Settings _settings = settings.Value;

    public override async Task<AuthenticateCommand> HandleAsync(AuthenticateCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        var account = await accountRepository.GetAccountByEmail(command.Email, cancellationToken);

        if (account == null || !account.IsVerified || !BC.Verify(command.Password, account.PasswordHash))
        {
            throw new UnauthorizedException();
        }

        var accessToken = tokenGenerator.GenerateAccessToken(account);
        var refreshToken = tokenGenerator.GenerateRefreshToken(ipAddressGetter.GetIPAddressFromRequest());

        await accountRepository.AddRefreshToken(refreshToken, account.Id, cancellationToken);
        await accountRepository.RemoveOldRefreshTokens(account, _settings.Security.RefreshTokenTTLInDays, cancellationToken);
        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_settings.Security.AccessTokenTTLInMinutes);
        var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(_settings.Security.RefreshTokenTTLInDays);

        command.Response = new TokenResponse
        {
            Account = account,
            AccessToken = accessToken,
            AccessTokenExpiry = accessTokenExpiry,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiry = refreshTokenExpiry
        };

        return await base.HandleAsync(command, cancellationToken);
    }
}
