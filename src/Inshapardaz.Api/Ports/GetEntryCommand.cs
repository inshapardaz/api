using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Paramore.Brighter;

namespace Inshapardaz.Api.Ports
{
    public class GetEntryRequest : IRequest
    {
        public Guid Id { get; set; }

        public EntryView Result { get; set; }

    }

    public class GetEntryCommandHandler : RequestHandlerAsync<GetEntryRequest>
    {
        private readonly IRenderResponse<EntryView> _entryRenderer;

        public GetEntryCommandHandler(IRenderResponse<EntryView> entryRenderer)
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
