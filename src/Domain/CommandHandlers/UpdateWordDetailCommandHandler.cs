using Darker;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using paramore.brighter.commandprocessor;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordDetailCommandHandler : RequestHandler<UpdateWordDetailCommand>
    {
        private readonly IDatabaseContext _database;
        private readonly IQueryProcessor _queryProcessor;

        public UpdateWordDetailCommandHandler(IDatabaseContext database,
            IQueryProcessor queryProcessor)
        {
            _database = database;
            _queryProcessor = queryProcessor;
        }

        public override UpdateWordDetailCommand Handle(UpdateWordDetailCommand command)
        {
            var detail = _queryProcessor.Execute(new WordDetailByIdQuery {Id =  command.WordDetail.Id}).WordDetail;

            if (detail == null)
            {
                throw new RecordNotFoundException();
            }

            detail.Attributes = command.WordDetail.Attributes;
            detail.Language = command.WordDetail.Language;

            _database.SaveChanges();

            return base.Handle(command);
        }
    }
}
