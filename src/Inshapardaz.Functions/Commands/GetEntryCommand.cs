using Inshapardaz.Functions.Converters;
using Inshapardaz.Functions.Views;
using Paramore.Brighter;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Commands
{
    public class GetEntryRequest : IRequest
    {
        public GetEntryRequest(ClaimsPrincipal user)
        {
            User = user;
        }

        public Guid Id { get; set; }

        public ClaimsPrincipal User { get; private set; }

        public EntryView Result { get; set; }
    }

    public class GetEntryRequestHandler : RequestHandlerAsync<GetEntryRequest>
    {
        public override async Task<GetEntryRequest> HandleAsync(GetEntryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = EntryRenderer.Render(command.User);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
