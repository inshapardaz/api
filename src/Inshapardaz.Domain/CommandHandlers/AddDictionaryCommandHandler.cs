using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddDictionaryCommandHandler : RequestHandlerAsync<AddDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public AddDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddDictionaryCommand> HandleAsync(AddDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _database.Dictionary.AddAsync(command.Dictionary, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}