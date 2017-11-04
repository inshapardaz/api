using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddTranslationCommandHandler : RequestHandlerAsync<AddTranslationCommand>
    {
        private readonly IDatabaseContext _database;

        public AddTranslationCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddTranslationCommand> HandleAsync(AddTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.Word.SingleOrDefaultAsync(
                t => t.Id == command.WordId && t.DictionaryId == command.DictioanryId, 
                cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            word.Translation.Add(command.Translation);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}