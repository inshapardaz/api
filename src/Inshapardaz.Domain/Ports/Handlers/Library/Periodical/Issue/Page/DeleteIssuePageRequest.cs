using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical.Issue.Page
{
    public class DeleteIssuePageRequest : LibraryBaseCommand
    {
        public DeleteIssuePageRequest(int libraryId,
                                int periodicalId,
                                int volumeNumber,
                                int issueNumber,
                                int sequenceNumber)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            SequenceNumber = sequenceNumber;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }

        public int SequenceNumber { get; }
    }

    public class DeleteIssuePageRequestHandler : RequestHandlerAsync<DeleteIssuePageRequest>
    {
        private readonly IIssuePageRepository _issuePageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteIssuePageRequestHandler(IIssuePageRepository issuePageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _issuePageRepository = issuePageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<DeleteIssuePageRequest> HandleAsync(DeleteIssuePageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            if (issuePage != null)
            {
                if (issuePage.ImageId.HasValue)
                {
                    var existingImage = await _fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
                    if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                    {
                        await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                    }

                    await _fileRepository.DeleteFile(existingImage.Id, cancellationToken);
                    await _issuePageRepository.DeletePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
                }

                await _issuePageRepository.DeletePage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
