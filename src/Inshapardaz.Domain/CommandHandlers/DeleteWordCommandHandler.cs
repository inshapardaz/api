using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

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
            var word = await _database.Word.SingleOrDefaultAsync(
                w => w.Id == command.WordId && w.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (word == null || word.Id != command.WordId)
            {
                throw new NotFoundException();
            }

            _database.Word.Remove(word);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}