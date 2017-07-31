using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordTranslationCommandHandler : RequestHandler<AddWordTranslationCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public AddWordTranslationCommandHandler(IDatabaseContext database, IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override AddWordTranslationCommand Handle(AddWordTranslationCommand command)
        {
            var detail = _queryProcessor.Execute(new WordDetailByIdQuery { Id = command.WordDetailId });
            detail.Translation.Add(command.Translation);

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}