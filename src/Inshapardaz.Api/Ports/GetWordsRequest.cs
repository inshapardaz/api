using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetWordsRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public PageView<WordView> Result { get; set; }
    }

    public class GetWordsRequestHandler : RequestHandlerAsync<GetWordsRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderWordPage _pageRenderer;

        public GetWordsRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderWordPage pageRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _pageRenderer = pageRenderer;
        }

        public override async Task<GetWordsRequest> HandleAsync(GetWordsRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _userHelper.GetUserId();
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (dictionary == null)
            {
                throw new NotFoundException();
            }

            if (!dictionary.IsPublic && dictionary.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            var query = new GetWordPageQuery
            {
                DictionaryId = command.DictionaryId,
                PageNumber = command.PageNumber,
                PageSize = command.PageSize
            };

            var results = await _queryProcessor.ExecuteAsync(query, cancellationToken);

            var pageRenderArgs = new PageRendererArgs<Word>
            {
                RouteName = "GetWords",
                Page = results
            };

            command.Result = _pageRenderer.Render(pageRenderArgs, command.DictionaryId);

            return command;
        }
    }
}
