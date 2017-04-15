using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordCommandHandler : RequestHandler<AddWordCommand>
    {
        private readonly IDatabase _database;

        public AddWordCommandHandler(IDatabase database)
        {
            _database = database;
        }

        public override AddWordCommand Handle(AddWordCommand command)
        {
            _database.Words.Add(command.Word);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
