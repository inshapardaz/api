using System.Threading;
using System.Threading.Tasks;
using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordTranslationCommandHandler : RequestHandlerAsync<DeleteWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteWordTranslationCommandHandler(
            IDatabaseContext database, IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override async Task<DeleteWordTranslationCommand> HandleAsync(DeleteWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var translation = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery { Id = command.TranslationId }, cancellationToken);

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