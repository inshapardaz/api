using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordCommandHandler : RequestHandler<DeleteWordCommand>
    {
        private readonly IDatabaseContext _database;

        public DeleteWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override DeleteWordCommand Handle(DeleteWordCommand command)
        {
            var w = _database.Words.SingleOrDefault(x => x.Id == command.Word.Id);

            if (w == null || w.Id != command.Word.Id)
            {
                throw new RecordNotFoundException();
            }

            _database.Words.Remove(w);

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
