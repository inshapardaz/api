using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class AddRelationshipRequest : DictionaryRequest
    {
        public AddRelationshipRequest(int dictionaryId, long wordId, WordRelation relationship)
            : base(dictionaryId)
        {
            WordId = wordId;
            Relationship = relationship;
        }

        public WordRelation Relationship { get; set; }

        public WordRelation Result { get; set; }

        public long WordId { get; set; }
    }

    public class AddRelationshipRequestHandler : RequestHandlerAsync<AddRelationshipRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IRelationshipRepository _relationshipRepository;

        public AddRelationshipRequestHandler(IWordRepository wordRepository, IRelationshipRepository relationshipRepository)
        {
            _wordRepository = wordRepository;
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddRelationshipRequest> HandleAsync(AddRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var sourceWord = await _wordRepository.GetWordById(command.DictionaryId, command.WordId, cancellationToken);
            if (sourceWord == null)
            {
                throw new NotFoundException();
            }

            var relatedWord = await _wordRepository.GetWordById(command.DictionaryId, command.Relationship.RelatedWordId, cancellationToken);
            if (relatedWord == null)
            {
                throw new BadRequestException();
            }

            if (sourceWord.DictionaryId != relatedWord.DictionaryId)
            {
                throw new BadRequestException();
            }

            var newRelationShip = await _relationshipRepository.AddRelationship(command.DictionaryId, command.Relationship, cancellationToken);
            newRelationShip.SourceWord = sourceWord;
            newRelationShip.RelatedWord = relatedWord;

            command.Result = newRelationShip;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
