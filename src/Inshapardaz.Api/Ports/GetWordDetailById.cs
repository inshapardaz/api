using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetWordDetailByIdRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long DetailId { get; set; }

        public WordDetailView Result { get; set; }
    }

    public class GetWordDetailByIdRequestHandler : RequestHandlerAsync<GetWordDetailByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderWordDetail _wordDetailRenderer;

        public GetWordDetailByIdRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderWordDetail wordDetailRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordDetailRenderer = wordDetailRenderer;
        }

        public override async Task<GetWordDetailByIdRequest> HandleAsync(GetWordDetailByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var details = await _queryProcessor.ExecuteAsync(new WordDetailByIdQuery { DictionaryId = command.DictionaryId, WordDetailId = command.DetailId }, cancellationToken);

            if (details == null)
            {
                throw new NotFoundException();
            }

            command.Result = _wordDetailRenderer.Render(details);
            return command;
        }
    }
}
