using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteTranslationRequest : DictionaryRequest
    {
        public DeleteTranslationRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public long TranslationId { get; set; }
    }

    public class DeleteTranslationRequestHandler : RequestHandlerAsync<DeleteTranslationRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;

        public DeleteTranslationRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteTranslationRequest> HandleAsync(DeleteTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _queryProcessor.ExecuteAsync(new GetTranslationByIdQuery(command.TranslationId), cancellationToken);

            if (response == null)
            {
                throw new BadRequestException();
            }

            await _commandProcessor.SendAsync(new DeleteWordTranslationCommand(command.DictionaryId, command.TranslationId), cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
