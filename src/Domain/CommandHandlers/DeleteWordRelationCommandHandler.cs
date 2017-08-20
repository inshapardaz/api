using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordRelationCommandHandler : RequestHandlerAsync<DeleteWordRelationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteWordRelationCommandHandler(
            IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<DeleteWordRelationCommand> HandleAsync(DeleteWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var relation = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = command.RelationId }, cancellationToken);

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