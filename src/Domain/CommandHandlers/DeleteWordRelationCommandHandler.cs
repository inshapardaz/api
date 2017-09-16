using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordRelationCommandHandler : RequestHandlerAsync<DeleteWordRelationCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordRelationCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<DeleteWordRelationCommand> HandleAsync(DeleteWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var relation = await _database.WordRelation.SingleOrDefaultAsync(r => r.Id == command.RelationId, cancellationToken);

            if (relation == null)
            {
                throw new RecordNotFoundException();
            }

            _database.WordRelation.Remove(relation);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}