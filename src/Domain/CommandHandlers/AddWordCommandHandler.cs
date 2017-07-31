using Inshapardaz.Domain.Commands;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordCommandHandler : RequestHandler<AddWordCommand>
    {
        private readonly IDatabaseContext _database;

        public AddWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override AddWordCommand Handle(AddWordCommand command)
        {
            _database.Word.Add(command.Word);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}