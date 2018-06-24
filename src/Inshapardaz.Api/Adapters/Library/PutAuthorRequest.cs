using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Library;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class PutAuthorRequest : RequestBase
    {
        public AuthorView Author { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public AuthorView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutAuthorRequestHandler : RequestHandlerAsync<PutAuthorRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public PutAuthorRequestHandler(IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        public override async Task<PutAuthorRequest> HandleAsync(PutAuthorRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            /*var result = await _queryProcessor.ExecuteAsync(new GetAuthorByIdQuery { AuthorId = command.AuthorId }, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            UpdateAuthorCommand updateAuthorCommand = new UpdateAuthorCommand
                (command.Author.Map<AuthorView, Domain.Entities.Author>());

            await _commandProcessor.SendAsync(updateAuthorCommand, cancellationToken: cancellationToken);*/

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}