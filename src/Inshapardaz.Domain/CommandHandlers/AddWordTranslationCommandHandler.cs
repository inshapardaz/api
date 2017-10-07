using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordTranslationCommandHandler : RequestHandlerAsync<AddWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordTranslationCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddWordTranslationCommand> HandleAsync(AddWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var detail = await _database.WordDetail.SingleOrDefaultAsync( wd => wd.Id == command.WordDetailId, cancellationToken);
            detail.Translation.Add(command.Translation);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}