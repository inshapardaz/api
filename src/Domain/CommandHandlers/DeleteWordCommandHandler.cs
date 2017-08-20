using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordCommandHandler : RequestHandlerAsync<DeleteWordCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<DeleteWordCommand> HandleAsync(DeleteWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var w = await _database.Word.SingleOrDefaultAsync(x => x.Id == command.WordId, cancellationToken);

            if (w == null || w.Id != command.WordId)
            {
                throw new RecordNotFoundException();
            }

            _database.Word.Remove(w);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}