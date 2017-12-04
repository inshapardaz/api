using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutRelationshipRequest : DictionaryRequest
    {
        public PutRelationshipRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public int RelationshipId { get; set; }

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

        public PutRelationshipRequestHandler(IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PutRelationshipRequest> HandleAsync(PutRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relation1 = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.RelationshipId), cancellationToken);

            if (relation1 == null)
            {
                throw new NotFoundException();
            }

            var relation2 = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.Relationship.SourceWordId), cancellationToken);

            if (relation2 == null)
            {
                throw new BadRequestException();
            }

            var dictionary2 = await _queryProcessor.ExecuteAsync(new DictionaryByWordIdQuery(command.Relationship.RelatedWordId), cancellationToken);
            if (dictionary2 == null || dictionary2.Id != command.DictionaryId)
            {
                throw new BadRequestException();
            }

            var updateCommand = new UpdateWordRelationCommand(command.DictionaryId, command.Relationship.Map<RelationshipView, WordRelation>());
            await _commandProcessor.SendAsync(updateCommand, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}