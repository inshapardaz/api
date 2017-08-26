using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordTranslationCommandHandler : RequestHandlerAsync<AddWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public AddWordTranslationCommandHandler(IDatabaseContext database, IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<AddWordTranslationCommand> HandleAsync(AddWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var detail = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { Id = command.WordDetailId }, cancellationToken);
            detail.Translation.Add(command.Translation);

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}