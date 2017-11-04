using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateDictionaryCommandHandler : RequestHandlerAsync<UpdateDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<UpdateDictionaryCommand> HandleAsync(UpdateDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existingDictionary = await _database.Dictionary.SingleOrDefaultAsync(d => d.Id == command.Dictionary.Id, cancellationToken);

            if (existingDictionary == null)
            {
                throw new NotFoundException();
            }

            existingDictionary.Name = command.Dictionary.Name;
            existingDictionary.Language = command.Dictionary.Language;
            existingDictionary.IsPublic = command.Dictionary.IsPublic;

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}