using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordDetailCommandHandler : RequestHandler<AddWordDetailCommand>
    {
        private readonly IDatabase _database;
        private readonly IQueryProcessor _queryProcessor;

        public AddWordDetailCommandHandler(IDatabase database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override AddWordDetailCommand Handle(AddWordDetailCommand command)
        {
            var word = _queryProcessor.Execute(new WordByIdQuery { Id = command.WordId });

            if (word == null)
            {
                throw new RecordNotFoundException();
            }

            word.Word.WordDetails.Add(command.WordDetail);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
