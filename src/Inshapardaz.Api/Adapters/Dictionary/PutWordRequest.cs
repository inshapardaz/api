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
    public class PutWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public WordView Word { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public int WordId { get; set; }

        public class RequestResult
        {
            public WordView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutWordRequestHandler : RequestHandlerAsync<PutWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;

        public PutWordRequestHandler(IQueryProcessor queryProcessor, 
                                      IAmACommandProcessor commandProcessor, 
                                      IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
        }

        public override async Task<PutWordRequest> HandleAsync(PutWordRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { WordId = command.WordId, UserId = userId }, cancellationToken);

            if (response == null)
            {
                throw new NotFoundException();
            }

            await _commandProcessor.SendAsync(new UpdateWordCommand { Word = command.Word.Map<WordView, Word>() }, cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
