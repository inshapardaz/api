using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class PutWordDetailRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long DetailId { get; set; }

        public WordDetailView WordDetail { get; set; }
    }

    public class PutWordDetailRequestHandler : RequestHandlerAsync<PutWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public PutWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<PutWordDetailRequest> HandleAsync(PutWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {

            var userId = _userHelper.GetUserId();
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (dictionary.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = command.DictionaryId, WordDetailId = command.DetailId }, cancellationToken);

            if (details == null || details.Id != command.WordDetail.Id)
            {
                throw new BadRequestException();
            }

            var updateWordDetailCommand = new UpdateWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordId = details.WordInstanceId,
                WordDetail = command.WordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(updateWordDetailCommand, cancellationToken: cancellationToken);

            return command;
        }
    }
}
