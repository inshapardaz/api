using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Account
{
    public class GetInvitationStatusQuery : IQuery<InvitationStatuses>
    {
        public GetInvitationStatusQuery(string invitationCode)
        {
            InvitationCode = invitationCode;
        }

        public string InvitationCode { get; }
    }

    public class GetInvitationStatusQueryHandler : QueryHandlerAsync<GetInvitationStatusQuery, InvitationStatuses>
    {
        private readonly IAccountRepository _accountRepository;

        public GetInvitationStatusQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override async Task<InvitationStatuses> ExecuteAsync(GetInvitationStatusQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var account = await _accountRepository.GetAccountByInvitationCode(query.InvitationCode, cancellationToken);

            if (account == null) return InvitationStatuses.NotFound;
            if (account.InvitationCodeExpiry < DateTime.UtcNow) return InvitationStatuses.Expired;
            return InvitationStatuses.Valid;
        }
    }
}
