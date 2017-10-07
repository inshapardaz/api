using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordCommandHandler : RequestHandlerAsync<UpdateWordCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<UpdateWordCommand> HandleAsync(UpdateWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var w = await _database.Word.SingleOrDefaultAsync(x => x.Id == command.Word.Id, cancellationToken);

            if (w == null || w.Id != command.Word.Id)
            {
                throw new RecordNotFoundException();
            }

            w.Title = command.Word.Title;
            w.TitleWithMovements = command.Word.TitleWithMovements;
            w.Pronunciation = command.Word.Pronunciation;
            w.Description = command.Word.Description;

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}