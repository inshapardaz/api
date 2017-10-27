using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetMeaningForWordRequest : IRequest
    {
        public Guid Id { get; set; }

        public long WordId { get; set; }

        public List<MeaningView> Result { get; set; }

        public int DictionaryId { get; set; }
    }

    public class GetMeaningForWordRequestHandler : RequestHandlerAsync<GetMeaningForWordRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningForWordRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        public override async Task<GetMeaningForWordRequest> HandleAsync(GetMeaningForWordRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO :  Check for dictionary access

            IEnumerable<Meaning> meanings = await _queryProcessor.ExecuteAsync(new WordMeaningByWordQuery
            {
                WordId = command.WordId
            }, cancellationToken);
            command.Result = meanings.Select(x => _meaningRenderer.Render(x)).ToList();
            return command;
        }
    }
}