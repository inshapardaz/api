using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Query.Account;

public class GetInvitationStatusQuery(string invitationCode) : IQuery<InvitationStatuses>
{
    public string InvitationCode { get; } = invitationCode;
}

public class GetInvitationStatusQueryHandler(IAccountRepository accountRepository)
    : QueryHandlerAsync<GetInvitationStatusQuery, InvitationStatuses>
{
    public override async Task<InvitationStatuses> ExecuteAsync(GetInvitationStatusQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        var account = await accountRepository.GetAccountByInvitationCode(query.InvitationCode, cancellationToken);

        if (account == null) return InvitationStatuses.NotFound;
        if (account.InvitationCodeExpiry < DateTime.UtcNow) return InvitationStatuses.Expired;
        return InvitationStatuses.Valid;
    }
}
