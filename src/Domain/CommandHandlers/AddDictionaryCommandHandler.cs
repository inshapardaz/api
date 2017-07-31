using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddDictionaryCommandHandler : RequestHandler<AddDictionaryCommand>
    {
        private readonly IDatabaseContext _database;

        public AddDictionaryCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override AddDictionaryCommand Handle(AddDictionaryCommand command)
        {
            _database.Dictionary.Add(command.Dictionary);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}