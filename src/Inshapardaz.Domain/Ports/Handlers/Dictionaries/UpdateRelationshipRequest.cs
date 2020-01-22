using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class UpdateRelationshipRequest : DictionaryRequest
    {
        public UpdateRelationshipRequest(int dictionaryId, long wordId, WordRelation relationship)
            : base(dictionaryId)
        {
            WordId = wordId;
            Relationship = relationship;
        }

        public long WordId { get; }

        public WordRelation Relationship { get; set; }

        public UpdateRelationshipResult Result { get; set; } = new UpdateRelationshipResult();

        public class UpdateRelationshipResult
        {
            public WordRelation Relationship { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateRelationshipRequestHandler : RequestHandlerAsync<UpdateRelationshipRequest>
    {
        private readonly IWordRepository _wordRepository;
        private readonly IRelationshipRepository _relationshipRepository;

        public UpdateRelationshipRequestHandler(IWordRepository wordRepository, IRelationshipRepository relationshipRepository)
        {
            _wordRepository = wordRepository;
            _relationshipRepository = relationshipRepository;
        }

        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<UpdateRelationshipRequest> HandleAsync(UpdateRelationshipRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var response = await _relationshipRepository.GetRelationshipById(command.DictionaryId, command.Relationship.Id, cancellationToken);

            if (response == null)
            {
                command.Relationship.Id = default(int);
                var newRelationship = await _relationshipRepository.AddRelationship(command.DictionaryId, command.Relationship, cancellationToken);
                command.Result.HasAddedNew = true;
                command.Result.Relationship = newRelationship;
            }
            else
            {
                await _relationshipRepository.UpdateRelationship(command.DictionaryId, command.Relationship, cancellationToken);
                command.Result.Relationship = command.Relationship;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
