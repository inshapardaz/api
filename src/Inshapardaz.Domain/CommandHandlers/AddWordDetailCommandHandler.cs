using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordDetailCommandHandler : RequestHandlerAsync<AddWordDetailCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordDetailCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<AddWordDetailCommand> HandleAsync(AddWordDetailCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var word = await _database.Word.SingleOrDefaultAsync(w => w.Id == command.WordId, cancellationToken);

            if (word == null)
            {
                throw new RecordNotFoundException();
            }

            word.WordDetail.Add(command.WordDetail);
            await _database.SaveChangesAsync(cancellationToken);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}