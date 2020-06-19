using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
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
    public class UpdatePeriodicalImageRequest : LibraryAuthorisedCommand
    {
        public UpdatePeriodicalImageRequest(ClaimsPrincipal claims, int libraryId, int periodicalId)
            : base(claims, libraryId)
        {
            PeriodicalId = periodicalId;
        }

        public int PeriodicalId { get; }

        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdatePeriodicalImageRequestHandler : RequestHandlerAsync<UpdatePeriodicalImageRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdatePeriodicalImageRequestHandler(IPeriodicalRepository PeriodicalRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _periodicalRepository = PeriodicalRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<UpdatePeriodicalImageRequest> HandleAsync(UpdatePeriodicalImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var periodical = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.PeriodicalId, cancellationToken);

            if (periodical == null)
            {
                throw new NotFoundException();
            }

            if (periodical.ImageId.HasValue)
            {
                command.Image.Id = periodical.ImageId.Value;
                var existingImage = await _fileRepository.GetFileById(periodical.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(periodical.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = periodical.ImageId.Value;
            }
            else
            {
                command.Image.Id = default(int);
                var url = await AddImageToFileStore(periodical.Id, command.Image.FileName, command.Image.Contents, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                periodical.ImageId = command.Result.File.Id;
                await _periodicalRepository.UpdatePeriodical(command.LibraryId, periodical, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int PeriodicalId, string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(PeriodicalId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(int PeriodicalId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"periodicals/{PeriodicalId}/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}
