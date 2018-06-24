using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class DeleteWordRelationshipByRelatedWordIdCommand : Command
    {
        public DeleteWordRelationshipByRelatedWordIdCommand(int dictionaryId, long relatedWordId)
        {
            DictionaryId = dictionaryId;
            RelatedWordId = relatedWordId;
        }

        public int DictionaryId { get; }

        public long RelatedWordId { get; }
    }

    public class DeleteWordRelationshipByRelatedWordIdCommandHandler : RequestHandlerAsync<DeleteWordRelationshipByRelatedWordIdCommand>
    {

        private readonly IRelationshipRepository _relationshipRepository;

        public DeleteWordRelationshipByRelatedWordIdCommandHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<DeleteWordRelationshipByRelatedWordIdCommand> HandleAsync(DeleteWordRelationshipByRelatedWordIdCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var relations = await _relationshipRepository.GetRelationshipToWord(command.DictionaryId, command.RelatedWordId, cancellationToken);
            foreach (var relation in relations)
            {
                await _relationshipRepository.DeleteRelationship(command.DictionaryId, relation.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}