using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
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
        private readonly IRelationshipRepository _relationshipRepository;

        public DeleteRelationshipRequestHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteRelationshipRequest> HandleAsync(DeleteRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _relationshipRepository.GetRelationshipById(command.DictionaryId, command.RelationshipId, cancellationToken);

            if (relations == null)
            {
                throw new NotFoundException();
            }

            await _relationshipRepository.DeleteRelationship(command.DictionaryId, command.RelationshipId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
