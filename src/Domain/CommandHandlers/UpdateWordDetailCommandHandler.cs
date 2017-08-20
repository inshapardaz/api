using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordDetailCommandHandler : RequestHandlerAsync<UpdateWordDetailCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordDetailCommandHandler(IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<UpdateWordDetailCommand> HandleAsync(UpdateWordDetailCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var detail = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = command.WordDetail.Id }, cancellationToken);

            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Attributes = command.WordDetail.Attributes;
            detail.Language = command.WordDetail.Language;

            await _database.SaveChangesAsync(cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
