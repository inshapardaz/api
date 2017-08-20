using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordRelationCommandHandler : RequestHandlerAsync<UpdateWordRelationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordRelationCommandHandler(
            IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<UpdateWordRelationCommand> HandleAsync(UpdateWordRelationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var relation = await _queryProcessor.ExecuteAsync(new RelationshipByIdQuery { Id = command.Relation.Id }, cancellationToken);
            if (relation == null)
            {
                throw new RecordNotFoundException();
            }

            relation.RelatedWordId = command.Relation.RelatedWordId;
            relation.SourceWordId = command.Relation.SourceWordId;
            relation.RelationType = command.Relation.RelationType;

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}