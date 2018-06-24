using System;
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
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.Queries.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutRelationshipRequest : DictionaryRequest
    {
        public PutRelationshipRequest(int dictionaryId, long wordId, int relationshipId)
            : base(dictionaryId)
        {
            WordId = wordId;
            RelationshipId = relationshipId;
        }

        public int RelationshipId { get; }

        public long WordId { get; }

        public RelationshipView Relationship { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public RelationshipView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutRelationshipRequestHandler : RequestHandlerAsync<PutRelationshipRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderRelation _relationRender;


        public PutRelationshipRequestHandler(IAmACommandProcessor commandProcessor, 
                                             IQueryProcessor queryProcessor, 
                                             IRenderRelation relationRender)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _relationRender = relationRender;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutRelationshipRequest> HandleAsync(PutRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relation1 = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.DictionaryId, command.Relationship.SourceWordId, command.RelationshipId), cancellationToken);

            if (relation1 == null)
            {
                throw new NotFoundException();
            }

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

            var wordRelation = command.Relationship.Map<RelationshipView, WordRelation>();
            var updateCommand = new UpdateWordRelationCommand(command.DictionaryId, wordRelation);
            await _commandProcessor.SendAsync(updateCommand, cancellationToken: cancellationToken);

            wordRelation.SourceWord = sourceWord;
            wordRelation.RelatedWord = relatedWord;
            var response = _relationRender.Render(wordRelation, command.DictionaryId, command.WordId);
            command.Result.Response = response;

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}