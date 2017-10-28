using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class GetDetailsByWordIdRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long WordId { get; set; }

        public List<WordDetailView> Result { get; set; }
    }

    public class GetDetailsByWordIdRequestHandler : RequestHandlerAsync<GetDetailsByWordIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public GetDetailsByWordIdRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderWordDetail wordDetailRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordDetailRenderer = wordDetailRenderer;
        }

        public override async Task<GetDetailsByWordIdRequest> HandleAsync(GetDetailsByWordIdRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var query = new WordDetailsByWordQuery
            {
                DictionaryId = command.DictionaryId,
                WordId = command.WordId
            };

            var wordDetailViews = await _queryProcessor.ExecuteAsync(query, cancellationToken);
            command.Result = wordDetailViews.Select(w => _wordDetailRenderer.Render(w)).ToList();
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}