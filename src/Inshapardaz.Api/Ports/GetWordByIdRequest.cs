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
    public class GetWordByIdRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }

        public long WordId { get; set; }

        public WordView Result { get; set; }
    }

    public class GetWordByIdRequestHandler : RequestHandlerAsync<GetWordByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderWord _wordRenderer;

        public GetWordByIdRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderWord wordRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _wordRenderer = wordRenderer;
        }

        public override async Task<GetWordByIdRequest> HandleAsync(GetWordByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
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

            var word = await _queryProcessor.ExecuteAsync(new WordByIdQuery { DictionaryId = command.DictionaryId, WordId = command.WordId, UserId = userId }, cancellationToken);
            if (word == null)
            {
                throw new NotFoundException();
            }

            command.Result = _wordRenderer.Render(word, command.DictionaryId);
            return command;
        }
    }
}
