using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordRelationCommandHandler : RequestHandlerAsync<UpdateWordRelationCommand>
    {
        private readonly IRelationshipRepository _relationshipRepository;

        public UpdateWordRelationCommandHandler(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
        }

        public override async Task<UpdateWordRelationCommand> HandleAsync(UpdateWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _relationshipRepository.UpdateRelationship(command.DictionaryId, command.Relation, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}