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
            var w = _database.Word.SingleOrDefault(x => x.Id == command.WordId);

            if (w == null || w.Id != command.WordId)
            {
                throw new RecordNotFoundException();
            }

            _database.Word.Remove(w);

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}