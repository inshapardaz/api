using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetRelationshipRequest : DictionaryRequest
    {
        public GetRelationshipRequest(int dictionaryId, long wordId, int relationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            RelationId = relationId;
        }

        public long WordId { get; }

        public int RelationId { get; }

        public WordRelation  Result { get; set; }
    }

    public class GetRelationshipRequestHandler : RequestHandlerAsync<GetRelationshipRequest>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public GetRelationshipRequestHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetRelationshipRequest> HandleAsync(GetRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var relations = await _relationshipRepository.GetRelationshipById(command.DictionaryId, command.RelationId, cancellationToken);

            if (relations == null)
            {
                throw new BadRequestException();
            }

            command.Result = relations;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
