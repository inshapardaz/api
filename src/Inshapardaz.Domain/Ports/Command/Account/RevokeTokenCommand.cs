using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Account;

public class RevokeTokenCommand(string token) : RequestBase
{
    public string Token { get; } = token;

    public TokenResponse Response { get; set; }
}

public class RevokeTokenCommandHandler(
    IAccountRepository accountRepository,
    IGetIPAddress ipAddressGetter,
    ILogger<RevokeTokenCommandHandler> logger,
    IUserHelper userHelper)
    : RequestHandlerAsync<RevokeTokenCommand>

{
    public override async Task<RevokeTokenCommand> HandleAsync(RevokeTokenCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            logger.LogInformation("Refresh token provided is null");
            throw new BadRequestException("Token is required");
        }

        if (!userHelper.IsAuthenticated)
        {
            return await base.HandleAsync(command, cancellationToken);
        }

        var refreshToken = await accountRepository.GetRefreshToken(command.Token, cancellationToken);
        var revoker = await accountRepository.GetAccountById(userHelper.AccountId.Value, cancellationToken);

        if (refreshToken != null)
        {
            if (refreshToken.AccountId != revoker.Id && !revoker.IsSuperAdmin)
            {
                logger.LogError("Unable to revoke token. Only admin and user themselves can revoke tokens");
                throw new UnauthorizedException();
            }

            var ipAddress = ipAddressGetter.GetIPAddressFromRequest();

            await accountRepository.RevokeRefreshToken(refreshToken.Token, ipAddress, null, cancellationToken);
            // Remove all access tokens for this user
        }
        else
        {
            logger.LogInformation("Refresh token provided is found");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
