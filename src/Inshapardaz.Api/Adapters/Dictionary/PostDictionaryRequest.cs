using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostDictionaryRequest : IRequest
    {
        public Guid Id { get; set; }

        public DictionaryView Dictionary { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public DictionaryView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostDictionaryRequestHandler : RequestHandlerAsync<PostDictionaryRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderDictionary _dictionaryRenderer;

        public PostDictionaryRequestHandler(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderDictionary dictionaryRenderer)
        {
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
            _dictionaryRenderer = dictionaryRenderer;
        }

        public override async Task<PostDictionaryRequest> HandleAsync(PostDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();

            var addDictionaryCommand = new AddDictionaryCommand
            {
                Dictionary = command.Dictionary.Map<DictionaryView, Domain.Database.Entities.Dictionary>()
            };

            addDictionaryCommand.Dictionary.UserId = userId;

            await _commandProcessor.SendAsync(addDictionaryCommand, cancellationToken: cancellationToken);

            var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary);

            command.Result.Response = response;
            command.Result.Location = response.Links.Single(x => x.Rel == RelTypes.Self).Href;

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
