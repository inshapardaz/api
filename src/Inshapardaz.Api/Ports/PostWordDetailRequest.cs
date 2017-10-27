using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class PostWordDetailRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long WordId { get; set; }

        public WordDetailView WordDetail { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public WordDetailView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PostWordDetailRequestHandler : RequestHandlerAsync<PostWordDetailRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public PostWordDetailRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderWordDetail wordDetailRenderer)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordDetailRenderer = wordDetailRenderer;
        }

        public override async Task<PostWordDetailRequest> HandleAsync(PostWordDetailRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var response = await _queryProcessor.ExecuteAsync(new WordByIdQuery { DictionaryId = command.DictionaryId, WordId = command.WordId }, cancellationToken);

            if (response == null)
            {
                throw new BadRequestException();
            }

            var addWordDetailCommand = new AddWordDetailCommand
            {
                DictionaryId = command.DictionaryId,
                WordId = command.WordId,
                WordDetail = command.WordDetail.Map<WordDetailView, WordDetail>()
            };

            await _commandProcessor.SendAsync(addWordDetailCommand, cancellationToken: cancellationToken);

            var responseView = _wordDetailRenderer.Render(addWordDetailCommand.WordDetail);
            command.Result.Location = responseView.Links.Single(x => x.Rel == "self").Href;
            command.Result.Response = responseView;

            return command;
        }
    }
}
