using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class DeleteIssueContentRequest : LibraryBaseCommand
    {
        public DeleteIssueContentRequest(int libraryId, int periodicalId, int volumeNumber, int issueNumber, string language, string mimeType)
            : base(libraryId)
        {
            PeriodicalId = periodicalId;
            VolumeNumber = volumeNumber;
            IssueNumber = issueNumber;
            Language = language;
            MimeType = mimeType;
        }

        public int PeriodicalId { get; }
        public int VolumeNumber { get; }
        public int IssueNumber { get; }
        public string Language { get; }
        public string MimeType { get; }
    }

    public class DeleteIssueContentRequestHandler : RequestHandlerAsync<DeleteIssueContentRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public DeleteIssueContentRequestHandler(IIssueRepository issueRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<DeleteIssueContentRequest> HandleAsync(DeleteIssueContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var content = await _issueRepository.GetIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Language, command.MimeType, cancellationToken);
            if (content != null)
            {
                await _fileStorage.TryDeleteFile(content.ContentUrl, cancellationToken);
                await _issueRepository.DeleteIssueContent(command.LibraryId, command.PeriodicalId, command.VolumeNumber, command.IssueNumber, command.Language, command.MimeType, cancellationToken);
                await _fileRepository.DeleteFile(content.FileId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
