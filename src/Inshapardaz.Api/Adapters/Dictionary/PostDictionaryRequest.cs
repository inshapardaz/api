using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Commands.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

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
        private readonly IQueryProcessor _queryProcessor;

        public PostDictionaryRequestHandler(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderDictionary dictionaryRenderer, IQueryProcessor queryProcessor)
        {
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
            _dictionaryRenderer = dictionaryRenderer;
            _queryProcessor = queryProcessor;
        }

        public override async Task<PostDictionaryRequest> HandleAsync(PostDictionaryRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();

            var addDictionaryCommand = new AddDictionaryCommand(
                ObjectMapper.Map<DictionaryView, Domain.Entities.Dictionary.Dictionary>(command.Dictionary)
            );

            addDictionaryCommand.Dictionary.UserId = userId;

            await _commandProcessor.SendAsync(addDictionaryCommand, cancellationToken: cancellationToken);
            var wordCount = await _queryProcessor.ExecuteAsync(new GetDictionaryWordCountQuery { DictionaryId = command.Dictionary.Id }, cancellationToken);

            var response = _dictionaryRenderer.Render(addDictionaryCommand.Dictionary, wordCount);

            command.Result.Response = response;
            command.Result.Location = response.Links.Single(x => x.Rel == RelTypes.Self).Href;

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
