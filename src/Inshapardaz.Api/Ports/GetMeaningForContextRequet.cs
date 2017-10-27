﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class GetMeaningForContextRequest : IRequest
    {
        public Guid Id { get; set; }

        public string Context { get; set; }

        public long WordId { get; set; }

        public IEnumerable<MeaningView> Result { get; set; }

        public int DictionaryId { get; set; }
    }

    public class GetMeaningForContextRequestHandler : RequestHandlerAsync<GetMeaningForContextRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderMeaning _meaningRenderer;

        public GetMeaningForContextRequestHandler(IQueryProcessor queryProcessor, IRenderMeaning meaningRenderer)
        {
            _queryProcessor = queryProcessor;
            _meaningRenderer = meaningRenderer;
        }

        public override async Task<GetMeaningForContextRequest> HandleAsync(GetMeaningForContextRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: check dictionary access
            var finalContext = string.Empty;
            if (command.Context != "default")
            {
                finalContext = command.Context;
            }

            var result = await _queryProcessor.ExecuteAsync(new WordMeaningByWordQuery
            {
                WordId = command.WordId,
                Context = finalContext
            }, cancellationToken);

            command.Result = result.Select(x => _meaningRenderer.Render(x)).ToList();
            return command;
        }
    }
}
