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
            var word = await _database.Word.SingleOrDefaultAsync(
                w => w.Id == command.Word.Id & w.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (word == null || word.Id != command.Word.Id)
            {
                throw new NotFoundException();
            }

            word.Title = command.Word.Title;
            word.TitleWithMovements = command.Word.TitleWithMovements;
            word.Pronunciation = command.Word.Pronunciation;
            word.Description = command.Word.Description;

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}