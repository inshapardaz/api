using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using paramore.brighter.commandprocessor;
using Command = Inshapardaz.Domain.Commands.Command;

namespace Inshapardaz.Api.Adapter
{
    public class GetEntryCommand : Command
    {
        public EntryView Result { get; set; }
    }

    public class GetEntryCommandHandler : RequestHandler<GetEntryCommand>
    {
        private readonly IRenderResponse<EntryView> _entryRenderer;

        public GetEntryCommandHandler(IActionContextAccessor a)
            //IRenderResponse<EntryView> entryRenderer)
        {
            //_entryRenderer = entryRenderer;
        }

        public override GetEntryCommand Handle(GetEntryCommand command)
        {
            return base.Handle(command);
        }

        //public override GetEntryCommand Handle(GetEntryCommand command, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    command.Result = _entryRenderer.Render();
        //    return await base.HandleAsync(command, cancellationToken);
        //}
    }
}
