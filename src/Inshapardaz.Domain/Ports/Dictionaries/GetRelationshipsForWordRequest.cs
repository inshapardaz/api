using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class GetRelationshipsForWordRequest : DictionaryRequest
    {
        public GetRelationshipsForWordRequest(int dictionaryId, long wordId)
            : base(dictionaryId)
        {
            WordId = wordId;
        }

        public IEnumerable<WordRelation> Result { get; set; }

        public long WordId { get; }
    }

    public class GetRelationshipsForWordRequestHandler : RequestHandlerAsync<GetRelationshipsForWordRequest>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public GetRelationshipsForWordRequestHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetRelationshipsForWordRequest> HandleAsync(GetRelationshipsForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Result = await _relationshipRepository.GetRelationshipFromWord(command.DictionaryId, command.WordId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
