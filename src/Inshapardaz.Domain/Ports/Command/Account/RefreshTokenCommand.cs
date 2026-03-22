using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class RefreshTokenCommand(string token) : RequestBase
{
    public string Token { get; } = token;

    public TokenResponse Response { get; set; }
}

public class RefreshTokenCommandHandler(
    IAccountRepository accountRepository,
    ILibraryRepository libraryRepository,
    IOptions<Settings> settings,
    IGenerateToken tokenGenerator,
    IGetIPAddress ipAddressGetter,
    ILogger<RefreshTokenCommandHandler> logger)
    : RequestHandlerAsync<RefreshTokenCommand>

{
    private readonly Settings _settings = settings.Value;

    public override async Task<RefreshTokenCommand> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Refreshing token");

        if (command.Token == null)
        {
            logger.LogInformation("Refresh token provided is null");
            throw new BadRequestException();
        }
        var refreshToken = await accountRepository.GetRefreshToken(command.Token, cancellationToken);
        if (refreshToken == null)
        {
            logger.LogInformation("Refresh token provided is invalid/not issued");
            throw new NotFoundException();
        }

        var account = await accountRepository.GetAccountById(refreshToken.AccountId, cancellationToken);
        if (account == null)
        {
            logger.LogInformation("Account related to Refresh token not found");
            throw new NotFoundException();
        }

        var ipAddress = ipAddressGetter.GetIPAddressFromRequest();

        var newRefreshToken = tokenGenerator.GenerateRefreshToken(ipAddress);

        await accountRepository.RevokeRefreshToken(refreshToken.Token, ipAddress, newRefreshToken.Token, cancellationToken);

        await accountRepository.RemoveOldRefreshTokens(account, _settings.Security.RefreshTokenTTLInDays, cancellationToken);

        var libraries = await libraryRepository.GetLibrariesByAccountId(account.Id, cancellationToken);
        var accessToken = tokenGenerator.GenerateAccessToken(account, libraries);

        var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_settings.Security.AccessTokenTTLInMinutes);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_settings.Security.RefreshTokenTTLInDays);

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
