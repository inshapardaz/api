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
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PostRelationshipRequest : DictionaryRequest
    {
        public PostRelationshipRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public RelationshipView Relationship { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public long WordId { get; set; }

        public class RequestResult
        {
            public RelationshipView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostRelationshipRequestHandler : RequestHandlerAsync<PostRelationshipRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderRelation _relationRender;
        
        public PostRelationshipRequestHandler(IAmACommandProcessor commandProcessor, 
            IQueryProcessor queryProcessor,
            IRenderRelation relationRender)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _relationRender = relationRender;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostRelationshipRequest> HandleAsync(PostRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var sourceWord = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.WordId), cancellationToken);
            if (sourceWord == null)
            {
                throw new NotFoundException();
            }

            var relatedWord = await _queryProcessor.ExecuteAsync(new GetWordByIdQuery(command.DictionaryId, command.Relationship.RelatedWordId), cancellationToken);
            if (relatedWord == null)
            {
                throw new BadRequestException();
            }

            if (sourceWord.DictionaryId != relatedWord.DictionaryId)
            {
                throw new BadRequestException();
            }

            var addCommand = new AddWordRelationCommand(
                command.DictionaryId,
                command.WordId,
                command.Relationship.RelatedWordId,
                (RelationType) command.Relationship.RelationTypeId
            );
            await _commandProcessor.SendAsync(addCommand, cancellationToken: cancellationToken);

            var result = addCommand.Result;
            result.SourceWord = sourceWord;
            result.RelatedWord = relatedWord;
            var response = _relationRender.Render(result, command.DictionaryId, command.WordId);
            command.Result.Location =  response.Links.Single(x => x.Rel == RelTypes.Self).Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
