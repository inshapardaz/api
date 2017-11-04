using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordMeaningCommandHandler : RequestHandlerAsync<UpdateWordMeaningCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateWordMeaningCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<UpdateWordMeaningCommand> HandleAsync(UpdateWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var meaning = await _database.Meaning.SingleOrDefaultAsync(
                m => m.Id == command.Meaning.Id && m.Word.DictionaryId == command.DictionaryId, 
                cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            meaning.Context = command.Meaning.Context;
            meaning.Value = command.Meaning.Value;
            meaning.Example = command.Meaning.Example;

            await _database.SaveChangesAsync(cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}