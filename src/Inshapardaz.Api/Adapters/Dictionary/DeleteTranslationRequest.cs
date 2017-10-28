using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Queries;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DeleteTranslationRequest : IRequest
    {
        public Guid Id { get; set; }

        public long TranslationId { get; set; }

        public int DictionaryId { get; set; }
    }

    public class DeleteTranslationRequestHandler : RequestHandlerAsync<DeleteTranslationRequest>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IUserHelper _userHelper;

        public DeleteTranslationRequestHandler(IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor, IUserHelper userHelper)
        {
            _commandProcessor = commandProcessor;
            _queryProcessor = queryProcessor;
            _userHelper = userHelper;
        }

        public override async Task<DeleteTranslationRequest> HandleAsync(DeleteTranslationRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var dictionary = await _queryProcessor.ExecuteAsync(new DictionaryByIdQuery
            {
                DictionaryId = command.DictionaryId
            }, cancellationToken);
            if (dictionary == null || dictionary.UserId != _userHelper.GetUserId())
            {
                throw new UnauthorizedAccessException();
            }

            var response = await _queryProcessor.ExecuteAsync(new TranslationByIdQuery
            {
                Id = command.TranslationId
            }, cancellationToken);

            if (response == null)
            {
                throw new BadRequestException();
            }

            await _commandProcessor.SendAsync(new DeleteWordTranslationCommand
            {
                TranslationId = command.TranslationId
            }, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
