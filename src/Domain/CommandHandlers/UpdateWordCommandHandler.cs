using System.Linq;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordCommandHandler : RequestHandler<UpdateWordCommand>
    {
        private readonly IDatabaseContext _database;

        public UpdateWordCommandHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override UpdateWordCommand Handle(UpdateWordCommand command)
        {
            var w = _database.Word.SingleOrDefault(x => x.Id == command.Word.Id);

            if (w == null || w.Id != command.Word.Id)
            {
                throw new RecordNotFoundException();
            }

            w.Title = command.Word.Title;
            w.TitleWithMovements = command.Word.TitleWithMovements;
            w.Pronunciation = command.Word.Pronunciation;
            w.Description = command.Word.Description;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}