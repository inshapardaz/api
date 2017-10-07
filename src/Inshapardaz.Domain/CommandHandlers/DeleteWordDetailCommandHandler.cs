using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordDetailCommandHandler : RequestHandlerAsync<DeleteWordDetailCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteWordDetailCommandHandler(IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<DeleteWordDetailCommand> HandleAsync(DeleteWordDetailCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = command.WordDetailId }, cancellationToken);

            if (details == null)
            {
                throw new RecordNotFoundException();
            }

            _database.WordDetail.Remove(details);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}