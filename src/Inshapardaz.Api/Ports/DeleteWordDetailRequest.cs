using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class DeleteWordDetailRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long DetailId { get; set; }
    }

    public class DeleteWordDetailRequestHandler : RequestHandlerAsync<DeleteWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public DeleteWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<DeleteWordDetailRequest> HandleAsync(DeleteWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException();
            }

            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery
            {
                DictionaryId = command.DictionaryId
            }, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (dictionary.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery
            {
                DictionaryId = command.DictionaryId,
                WordDetailId = command.DetailId
            }, cancellationToken);

            if (details == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordDetailId = command.DetailId
            }, cancellationToken: cancellationToken);

            return command;
        }
    }
}
