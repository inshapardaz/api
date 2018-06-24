using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Commands.Dictionary
{
    public class UpdateWordRelationCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordRelationCommand(int dictionaryId, WordRelation relation)
        {
            DictionaryId = dictionaryId;
            Relation = relation ?? throw new ArgumentNullException(nameof(relation));
        }

        public WordRelation Relation { get; }
    }

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