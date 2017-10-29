using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
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
            var word = await _database.Word.SingleOrDefaultAsync( wd => wd.Id == command.WordId, cancellationToken);
            word.Translation.Add(command.Translation);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}