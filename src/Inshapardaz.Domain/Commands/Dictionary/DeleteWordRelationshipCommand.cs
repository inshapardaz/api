using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class DeleteWordRelationshipCommand : Command
    {
        public DeleteWordRelationshipCommand(int dictionaryId, long relationshipId)
        {
            DictionaryId = dictionaryId;
            RelationshipId = relationshipId;
        }

        public int DictionaryId { get; }

        public long RelationshipId { get; }
    }

    public class DeleteWordRelationshipCommandHandler : RequestHandlerAsync<DeleteWordRelationshipCommand>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public DeleteWordRelationshipCommandHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<DeleteWordRelationshipCommand> HandleAsync(DeleteWordRelationshipCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _relationshipRepository.DeleteRelationship(command.DictionaryId, command.RelationshipId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}