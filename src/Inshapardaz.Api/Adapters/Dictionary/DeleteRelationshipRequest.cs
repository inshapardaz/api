using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteRelationshipRequest : DictionaryRequest
    {
        public DeleteRelationshipRequest(int dictionaryId, long wordId, int relationshipId)
            : base(dictionaryId)
        {
            WordId = wordId;
            RelationshipId = relationshipId;
        }

        public long WordId { get; }

        public int RelationshipId { get; }
    }

    public class DeleteRelationshipRequestHandler : RequestHandlerAsync<DeleteRelationshipRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteRelationshipRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteRelationshipRequest> HandleAsync(DeleteRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _queryProcessor.ExecuteAsync(new GetRelationshipByIdQuery(command.DictionaryId, command.WordId, command.RelationshipId), cancellationToken);

            if (relations == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordRelationshipCommand(command.DictionaryId, command.RelationshipId), cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
