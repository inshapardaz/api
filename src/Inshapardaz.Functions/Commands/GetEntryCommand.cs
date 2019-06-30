using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Adapters;
using Inshapardaz.Functions.Views;
using Microsoft.IdentityModel.Tokens;
using Paramore.Brighter;

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
        private readonly IRenderEntry _entryRenderer;

        public GetEntryRequestHandler(IRenderEntry entryRenderer)
        {
            _entryRenderer = entryRenderer;
        }

        public override async Task<GetEntryRequest> HandleAsync(GetEntryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = _entryRenderer.Render(command.User);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}