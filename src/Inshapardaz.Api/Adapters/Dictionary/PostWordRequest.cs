using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public WordView Word { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public WordView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostWordRequestHandler : RequestHandlerAsync<PostWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderWord _wordRenderer;

        public PostWordRequestHandler(IQueryProcessor queryProcessor, 
                                      IAmACommandProcessor commandProcessor, 
                                      IUserHelper userHelper,
                                      IRenderWord wordRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordRenderer = wordRenderer;
            _commandProcessor = commandProcessor;
        }

        public override async Task<PostWordRequest> HandleAsync(PostWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId, UserId = userId }, cancellationToken);

            if (userId == null || dictionary == null)
            {
                throw new UnauthorizedAccessException();
            }

            var addWordCommand = new AddWordCommand { DictionaryId = command.DictionaryId, Word = command.Word.Map<WordView, Word>() };
            addWordCommand.Word.DictionaryId = command.DictionaryId;
            await _commandProcessor.SendAsync(addWordCommand, cancellationToken: cancellationToken);

            command.Result.Response = _wordRenderer.Render(addWordCommand.Word, command.DictionaryId);
            command.Result.Location = command.Result.Response.Links.Single(x => x.Rel == "self").Href;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
