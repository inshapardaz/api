using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordTranslationCommandHandler : RequestHandlerAsync<DeleteWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordTranslationCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<DeleteWordTranslationCommand> HandleAsync(DeleteWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var translation = await _database.Translation.SingleOrDefaultAsync( t => t.Id == command.TranslationId, cancellationToken);

            if (translation == null)
            {
                throw new RecordNotFoundException();
            }

            _database.Translation.Remove(translation);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}