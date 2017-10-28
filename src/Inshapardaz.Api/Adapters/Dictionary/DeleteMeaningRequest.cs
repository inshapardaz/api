using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteMeaningRequest : IRequest
    {
        public Guid Id { get; set; }

        public long MeaningId { get; set; }

        public int DictionaryId { get; set; }
    }

    public class DeleteMeaningRequestHandler : RequestHandlerAsync<DeleteMeaningRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public DeleteMeaningRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<DeleteMeaningRequest> HandleAsync(DeleteMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery
            {
                DictionaryId = command.DictionaryId
            }, cancellationToken);
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            var response = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery
            {
                MeaningId = command.MeaningId
            }, cancellationToken);

            if (response == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new DeleteWordMeaningCommand
            {
                MeaningId = response.Id
            }, cancellationToken: cancellationToken);


            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
