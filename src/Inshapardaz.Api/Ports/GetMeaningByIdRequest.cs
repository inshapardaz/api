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

namespace Inshapardaz.Api.Ports
{
    public class GetMeaningByIdRequest : IRequest
    {
        public Guid Id { get; set; }

        public long MeaningId { get; set; }

        public MeaningView Result { get; set; }

        public int DictionaryId { get; set; }
    }

    public class GetMeaningByIdRequestHandler : RequestHandlerAsync<GetMeaningByIdRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningByIdRequestHandler(IQueryProcessor queryProcessor, IUserHelper userHelper, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
            _meaningRenderer = meaningRenderer;
        }

        public override async Task<GetMeaningByIdRequest> HandleAsync(GetMeaningByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            // TODO: Check dictionary access
            var user = _userHelper.GetUserId();
            if (user != Guid.Empty)
            {
                var dictionaryQuery = new DictionaryByIdQuery
                {
                    DictionaryId = command.DictionaryId
                };
                var dictionary = await _queryProcessor.ExecuteAsync(dictionaryQuery, cancellationToken);
                if (dictionary != null && dictionary.UserId != user)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            var meaning = await _queryProcessor.ExecuteAsync(new WordMeaningByIdQuery { MeaningId = command.MeaningId }, cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            command.Result = _meaningRenderer.Render(meaning);
            return command;
        }
    }
}
