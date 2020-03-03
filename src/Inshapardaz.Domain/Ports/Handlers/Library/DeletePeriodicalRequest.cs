using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class DeletePeriodicalRequest : LibraryAuthorisedCommand
    {
        public DeletePeriodicalRequest(ClaimsPrincipal claims, int libraryId, int periodicalId)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
        }

        public int PeriodicalId { get; }
    }

    public class DeletePeriodicalRequestHandler : RequestHandlerAsync<DeletePeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeletePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _periodicalRepository = periodicalRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<DeletePeriodicalRequest> HandleAsync(DeletePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);
            if (periodical != null)
            {
                await _periodicalRepository.DeletePeriodical(command.LibraryId, command.PeriodicalId, cancellationToken);

                if (!string.IsNullOrWhiteSpace(periodical.ImageUrl))
                {
                    await _fileStorage.TryDeleteImage(periodical.ImageUrl, cancellationToken);
                    await _fileRepository.DeleteFile(periodical.ImageId.Value, cancellationToken);
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
