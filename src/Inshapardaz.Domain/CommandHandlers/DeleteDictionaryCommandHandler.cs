using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteDictionaryCommandHandler : RequestHandlerAsync<DeleteDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<DeleteDictionaryCommand> HandleAsync(DeleteDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var d = await _database.Dictionary.SingleOrDefaultAsync(x => x.Id == command.DictionaryId && x.UserId == command.UserId, cancellationToken);

            if (d == null || d.Id != command.DictionaryId)
            {
                throw new RecordNotFoundException();
            }

            _database.Dictionary.Remove(d);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}