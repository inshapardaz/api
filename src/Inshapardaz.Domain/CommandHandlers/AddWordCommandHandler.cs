using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordCommandHandler : RequestHandlerAsync<AddWordCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddWordCommand> HandleAsync(AddWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command.Word == null)
            {
                throw new BadRequestException();
            }

            var dictionary = await _database.Dictionary.SingleOrDefaultAsync(d => d.Id == command.DictionaryId, cancellationToken: cancellationToken);
            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            dictionary.Word.Add(command.Word);
            await _database.SaveChangesAsync(cancellationToken);

            return await  base.HandleAsync(command, cancellationToken);
        }
    }
}