using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.Renderers.Dictionary;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Helpers;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Library
{
    public class GetAuthorByIdRequest : RequestBase
    {
        public GetAuthorByIdRequest(int authorId)
        {
            AuthorId = authorId;
        }

        public AuthorView Result { get; set; }
        public int AuthorId { get; }
    }

    public class GetDictionaryByIdRequestHandler : RequestHandlerAsync<GetAuthorByIdRequest>
    {
        private readonly IRenderDictionary _dictionaryRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IQueryProcessor _queryProcessor;

        public GetDictionaryByIdRequestHandler(IQueryProcessor queryProcessor, 
                                               IRenderDictionary dictionaryRenderer,
                                               IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _dictionaryRenderer = dictionaryRenderer;
            _userHelper = userHelper;
        }

        public override async Task<GetAuthorByIdRequest> HandleAsync(GetAuthorByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            /*var result = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery { DictionaryId = command.DictionaryId }, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException();
            }

            var wordCount = await _queryProcessor.ExecuteAsync(new GetDictionaryWordCountQuery {DictionaryId = command.DictionaryId}, cancellationToken);
            command.Result = _dictionaryRenderer.Render(result, wordCount);*/
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
