using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordTranslationCommandHandler : RequestHandler<UpdateWordTranslationCommand>
    {
        private readonly IDatabase _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordTranslationCommandHandler(
            IDatabase database, 
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override UpdateWordTranslationCommand Handle(UpdateWordTranslationCommand command)
        {
            var translation = _queryProcessor.Execute(new TranslationByIdQuery {Id = command.Translation.Id});

            if (translation.Translation == null)
            {
                throw new RecordNotFoundException();
            }

            translation.Translation.Language = command.Translation.Language;
            translation.Translation.Value = command.Translation.Value;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
