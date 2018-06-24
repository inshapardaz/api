using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers.Library;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Helpers;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class PostAuthorRequest : RequestBase
    {
        public AuthorView Author { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public AuthorView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostAuthorRequestHandler : RequestHandlerAsync<PostAuthorRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IRenderAuthor _authorRenderer;
        private readonly IQueryProcessor _queryProcessor;

        public PostAuthorRequestHandler(IAmACommandProcessor commandProcessor, IUserHelper userHelper, IRenderAuthor authorRenderer, IQueryProcessor queryProcessor)
        {
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
            _authorRenderer = authorRenderer;
            _queryProcessor = queryProcessor;
        }

        public override async Task<PostAuthorRequest> HandleAsync(PostAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
           /* var userId = _userHelper.GetUserId();

            var addAuthorCommand = new AddAuthorCommand(
                command.Author.Map<AuthorView, Domain.Entities.Author>()
            );

            addAuthorCommand.Author.UserId = userId;

            await _commandProcessor.SendAsync(addAuthorCommand, cancellationToken: cancellationToken);
            var wordCount = await _queryProcessor.ExecuteAsync(new GetAuthorWordCountQuery { AuthorId = command.Author.Id }, cancellationToken);

            var response = _authorRenderer.Render(addAuthorCommand.Author, wordCount);

            command.Result.Response = response;
            command.Result.Location = response.Links.Single(x => x.Rel == RelTypes.Self).Href;*/

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
