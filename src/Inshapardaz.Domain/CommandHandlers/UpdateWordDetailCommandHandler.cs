using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordDetailCommandHandler : RequestHandlerAsync<UpdateWordDetailCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateWordDetailCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override async Task<UpdateWordDetailCommand> HandleAsync(UpdateWordDetailCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var detail = await _database.WordDetail.SingleOrDefaultAsync(wd => wd.Id == command.WordDetail.Id, cancellationToken);

            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Attributes = command.WordDetail.Attributes;
            detail.Language = command.WordDetail.Language;

            await _database.SaveChangesAsync(cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
