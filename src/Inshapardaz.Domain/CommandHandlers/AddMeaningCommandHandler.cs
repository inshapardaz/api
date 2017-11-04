using Inshapardaz.Domain.Commands;
using Paramore.Brighter;
using Inshapardaz.Domain.Exception;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddMeaningCommandHandler : RequestHandlerAsync<AddMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public AddMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddMeaningCommand> HandleAsync(AddMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command.Meaning == null)
            {
                throw new BadRequestException();
            }

            var word = await _database.Word.SingleOrDefaultAsync(
                w => w.Id == command.WordId && w.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (word == null)
            {
                throw new NotFoundException();
            }

            word.Meaning.Add(command.Meaning);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}