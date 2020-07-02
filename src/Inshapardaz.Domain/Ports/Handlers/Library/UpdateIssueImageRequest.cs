using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateIssueImageRequest : LibraryAuthorisedCommand
    {
        public UpdateIssueImageRequest(ClaimsPrincipal claims, int libraryId, int periodicalId, int issueId)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
            IssueId = issueId;
        }

        public int PeriodicalId { get; private set; }

        public int IssueId { get; }

        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateIssueImageRequestHandler : RequestHandlerAsync<UpdateIssueImageRequest>
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateIssueImageRequestHandler(IIssueRepository issueRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _issueRepository = issueRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdateIssueImageRequest> HandleAsync(UpdateIssueImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssueById(command.LibraryId, command.PeriodicalId, command.IssueId, cancellationToken);

            if (issue == null)
            {
                throw new NotFoundException();
            }

            if (issue.ImageId.HasValue)
            {
                command.Image.Id = issue.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(issue.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(command.PeriodicalId, issue.Id, command.Image.FileName, command.Image.Contents, cancellationToken);

                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = issue.ImageId.Value;
            }
            else
            {
                command.Image.Id = default;
                var url = await AddImageToFileStore(command.PeriodicalId, issue.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                issue.ImageId = command.Result.File.Id;
                await _issueRepository.UpdateIssue(command.LibraryId, command.PeriodicalId, issue, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int periodicalId, int issueId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(periodicalId, issueId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int periodicalId, int issueId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"periodicals/{periodicalId}/issues/{issueId}/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
