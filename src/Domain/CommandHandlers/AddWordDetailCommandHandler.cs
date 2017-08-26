using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordDetailCommandHandler : RequestHandlerAsync<AddWordDetailCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public AddWordDetailCommandHandler(IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<AddWordDetailCommand> HandleAsync(AddWordDetailCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { Id = command.WordId });

            if (word == null)
            {
                throw new RecordNotFoundException();
            }

            word.WordDetail.Add(command.WordDetail);
            await _database.SaveChangesAsync(cancellationToken);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}