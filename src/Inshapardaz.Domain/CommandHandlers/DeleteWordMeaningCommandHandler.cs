using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordMeaningCommandHandler : RequestHandlerAsync<DeleteWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<DeleteWordMeaningCommand> HandleAsync(DeleteWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning = await _database.Meaning.SingleOrDefaultAsync(x => x.Id == command.MeaningId, cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            _database.Meaning.Remove(meaning);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}