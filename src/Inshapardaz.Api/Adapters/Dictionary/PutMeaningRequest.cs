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

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class PutMeaningRequest : IRequest
    {
        public Guid Id { get; set; }

        public long MeaningId { get; set; }

        public MeaningView Meaning { get; set; }

        public int DictionaryId { get; set; }
    }

    public class PutMeaningRequestHandler : RequestHandlerAsync<PutMeaningRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IUserHelper _userHelper;

        public PutMeaningRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
        }

        public override async Task<PutMeaningRequest> HandleAsync(PutMeaningRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            if (response == null || response.Id != command.Meaning.Id)
            {
                throw new BadRequestException();
            }

            await _commandProcessor.SendAsync(new UpdateWordMeaningCommand
            {
                Meaning = command.Meaning.Map<MeaningView, Meaning>()
            }, cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
