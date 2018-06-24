using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class DeleteTranslationRequest : DictionaryRequest
    {
        public DeleteTranslationRequest(int dictionaryId, long wordId, int translationId)
            : base(dictionaryId)
        {
            WordId = wordId;
            TranslationId = translationId;
        }

        public long WordId { get; }

        public int TranslationId { get; set; }
    }

    public class DeleteTranslationRequestHandler : RequestHandlerAsync<DeleteTranslationRequest>
    {
        private readonly ITranslationRepository _translationRepository;

        public DeleteTranslationRequestHandler(ITranslationRepository translationRepository)
        {
            _translationRepository = translationRepository;
        }
        [DictionaryWriteRequestValidation(1, HandlerTiming.Before)]
        public override async Task<DeleteTranslationRequest> HandleAsync(DeleteTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _translationRepository.GetTranslationById(command.DictionaryId, command.WordId, command.TranslationId, cancellationToken);

            if (response == null)
            {
                throw new BadRequestException();
            }

            await _translationRepository.DeleteTranslation(command.DictionaryId, command.WordId, command.TranslationId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
