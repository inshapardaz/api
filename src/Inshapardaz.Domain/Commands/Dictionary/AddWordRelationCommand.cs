using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class AddWordRelationCommand : Command
    {
        public AddWordRelationCommand(int dictionaryId, long sourceWordId, long relatedWordId, RelationType relationType)
        {
            DictionaryId = dictionaryId;
            SourceWordId = sourceWordId;
            RelatedWordId = relatedWordId;
            RelationType = relationType;
        }

        public int DictionaryId { get; }

        public long SourceWordId { get; }
        public long RelatedWordId { get; }
        public RelationType RelationType { get; }

        public WordRelation Result { get; set; }
    }

    public class AddWordRelationCommandHandler : RequestHandlerAsync<AddWordRelationCommand>
    {
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IWordRepository _wordRepository;
        public AddWordRelationCommandHandler(IWordRepository wordRepository, IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
            _wordRepository = wordRepository;
        }

        public override async Task<AddWordRelationCommand> HandleAsync(AddWordRelationCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            if (command.SourceWordId == command.RelatedWordId)
            {
                throw new BadRequestException();
            }

            var source = await _wordRepository.GetWordById(command.DictionaryId, command.SourceWordId, cancellationToken);
            var related = await _wordRepository.GetWordById(command.DictionaryId, command.RelatedWordId, cancellationToken);

            if (source == null || related == null)
            {
                throw new NotFoundException();
            }

            var relation = new WordRelation
            {
                SourceWordId = command.SourceWordId,
                RelatedWordId = command.RelatedWordId,
                RelationType = command.RelationType
            };

            command.Result = await _relationshipRepository.AddRelationship(command.DictionaryId, relation, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}