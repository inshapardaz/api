using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordDetailCommandHandler : RequestHandler<DeleteWordDetailCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteWordDetailCommandHandler(IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override DeleteWordDetailCommand Handle(DeleteWordDetailCommand command)
        {
            var details = _queryProcessor.Execute(new WordDetailByIdQuery { Id = command.WordDetailId });


            if (details == null)
            {
                throw new RecordNotFoundException();
            }

            _database.WordDetails.Remove(details);
            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
