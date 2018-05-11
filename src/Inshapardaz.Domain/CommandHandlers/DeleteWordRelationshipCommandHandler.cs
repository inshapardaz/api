using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
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