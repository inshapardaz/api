using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddDictionaryCommandHandler : RequestHandler<AddDictionaryCommand>
    {
        private readonly IDatabase _database;

        public AddDictionaryCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public override AddDictionaryCommand Handle(AddDictionaryCommand command)
        {
            _database.Dictionaries.Add(command.Dictionary);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
