using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordTranslationCommandHandler : RequestHandlerAsync<UpdateWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordTranslationCommandHandler(
            IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<UpdateWordTranslationCommand> HandleAsync(UpdateWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var translation = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery { Id = command.Translation.Id }, cancellationToken);

            if (translation == null)
            {
                throw new RecordNotFoundException();
            }

            translation.Language = command.Translation.Language;
            translation.Value = command.Translation.Value;

            await _database.SaveChangesAsync(cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
