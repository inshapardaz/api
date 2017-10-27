using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Ports
{
    public class PutTranslationRequest : IRequest
    {
        public Guid Id { get; set; }
        public int DictionaryId { get; set; }
        public int TranslationId { get; set; }
        public TranslationView Translation { get; set; }
        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public TranslationView Response { get; set; }

            public Uri Location { get; set; }
        }
    }

    public class PutTranslationRequestHandler : RequestHandlerAsync<PutTranslationRequest>
    {
        private readonly IUserHelper _userHelper;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderDictionary _dictionaryRenderer;
        
        public PutTranslationRequestHandler(IUserHelper userHelper, 
                                           IAmACommandProcessor commandProcessor, 
                                           IQueryProcessor queryProcessor,
                                           IRenderDictionary dictionaryRenderer)
        {
            _userHelper = userHelper;
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _dictionaryRenderer = dictionaryRenderer;
        }
        public override async Task<PutTranslationRequest> HandleAsync(PutTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery { DictionaryId = command.DictionaryId });
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            var response = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery { Id = command.TranslationId });

            if (response == null)
            {
                throw new BadImageFormatException();
            }

            await _commandProcessor.SendAsync(new UpdateWordTranslationCommand { Translation = command.Translation.Map<TranslationView, Translation>() });

            return command;
        }
    }
}