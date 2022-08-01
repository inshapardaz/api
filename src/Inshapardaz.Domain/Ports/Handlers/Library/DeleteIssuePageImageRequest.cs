using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteIssuePageImageRequest : LibraryBaseCommand
    {
        public DeleteIssuePageImageRequest(int libraryId, 
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

    public class DeleteIssuePageImageRequestHandler : RequestHandlerAsync<DeleteIssuePageImageRequest>
    {
        private readonly IIssuePageRepository _issuePageRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteIssuePageImageRequestHandler(IIssuePageRepository issuePageRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _issuePageRepository = issuePageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteIssuePageImageRequest> HandleAsync(DeleteIssuePageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issuePage = await _issuePageRepository.GetPageBySequenceNumber(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);

            if (issuePage != null && issuePage.ImageId.HasValue)
            {
                var existingImage = await _fileRepository.GetFileById(issuePage.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                await _fileRepository.DeleteFile(existingImage.Id, cancellationToken);
                await _issuePageRepository.DeletePageImage(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.SequenceNumber, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
