using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
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

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
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
                throw new NotFoundException();
            }

            var dictionary2 = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery(command.Relationship.RelatedWordId), cancellationToken);
            if (dictionary2 == null || dictionary2.Id != command.DictionaryId)
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

            var newRelationship = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.DictionaryId, command.WordId, command.Relationship.Id), cancellationToken);
            var response = _relationRender.Render(newRelationship, command.DictionaryId, command.WordId);
            command.Result.Location =  response.Links.Single(x => x.Rel == RelTypes.Self).Href;
            command.Result.Response =  response;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
