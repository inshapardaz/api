using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;
using Inshapardaz.Domain.Exception;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordMeaningCommandHandler : RequestHandlerAsync<AddWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddWordMeaningCommand> HandleAsync(AddWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var detail = await _database.WordDetail.SingleOrDefaultAsync(w => w.Id == command.WordDetailId, cancellationToken);
            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Meaning.Add(command.Meaning);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}