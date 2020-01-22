using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using FileModel = Inshapardaz.Domain.Models.FileModel;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateIssueImageRequest : RequestBase
    {
        public UpdateIssueImageRequest(int periodicalId, int issueId)
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

        public override async Task<UpdateIssueImageRequest> HandleAsync(UpdateIssueImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var issue = await _issueRepository.GetIssueById(command.PeriodicalId, command.IssueId, cancellationToken);

            if (issue == null)
            {
                throw new NotFoundException();
            }

            if (issue.ImageId.HasValue)
            {
                command.Image.Id = issue.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(issue.ImageId.Value, true, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteFile(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(command.PeriodicalId, issue.Id, command.Image.FileName, command.Image.Contents, cancellationToken);

                await _fileRepository.UpdateFile(command.Image, url, true, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = issue.ImageId.Value;
            }
            else
            {
                command.Image.Id = default(int);
                var url = await AddImageToFileStore(command.PeriodicalId, issue.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Result.File = await _fileRepository.AddFile(command.Image, url, true, cancellationToken);
                command.Result.HasAddedNew = true;

                issue.ImageId = command.Result.File.Id;
                await _issueRepository.UpdateIssue(command.PeriodicalId, issue, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int periodicalId, int issueId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(periodicalId, issueId, fileName);
            return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int periodicalId, int issueId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"periodicals/{periodicalId}/issues/{issueId}/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
