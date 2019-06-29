using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Adapters;
using Inshapardaz.Functions.Views;
using Paramore.Brighter;

namespace Inshapardaz.Functions.Commands
{
    public class GetEntryRequest : IRequest
    {
        public Guid Id { get; set; }

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
            command.Result = _entryRenderer.Render();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}