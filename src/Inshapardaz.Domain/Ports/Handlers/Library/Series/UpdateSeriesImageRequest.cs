using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Series
{
    public class UpdateSeriesImageRequest : LibraryBaseCommand
    {
        public UpdateSeriesImageRequest(int libraryId, int seriesId)
            : base(libraryId)
        {
            SeriesId = seriesId;
        }

        public int SeriesId { get; }

        public FileModel Image { get; set; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public FileModel File { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateSeriesImageRequestHandler : RequestHandlerAsync<UpdateSeriesImageRequest>
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public UpdateSeriesImageRequestHandler(ISeriesRepository seriesRepository, IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _seriesRepository = seriesRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<UpdateSeriesImageRequest> HandleAsync(UpdateSeriesImageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var series = await _seriesRepository.GetSeriesById(command.LibraryId, command.SeriesId, cancellationToken);

            if (series == null)
            {
                throw new NotFoundException();
            }

            if (series.ImageId.HasValue)
            {
                command.Image.Id = series.ImageId.Value;

                var existingImage = await _fileRepository.GetFileById(series.ImageId.Value, cancellationToken);
                if (existingImage != null && !string.IsNullOrWhiteSpace(existingImage.FilePath))
                {
                    await _fileStorage.TryDeleteImage(existingImage.FilePath, cancellationToken);
                }

                var url = await AddImageToFileStore(series.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                await _fileRepository.UpdateFile(command.Image, cancellationToken);
                command.Result.File = command.Image;
                command.Result.File.Id = series.ImageId.Value;
            }
            else
            {
                command.Image.Id = default;
                var url = await AddImageToFileStore(series.Id, command.Image.FileName, command.Image.Contents, command.Image.MimeType, cancellationToken);
                command.Image.FilePath = url;
                command.Image.IsPublic = true;
                command.Result.File = await _fileRepository.AddFile(command.Image, cancellationToken);
                command.Result.HasAddedNew = true;

                await _seriesRepository.UpdateSeriesImage(command.LibraryId, command.SeriesId, command.Result.File.Id, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(int seriesId, string fileName, byte[] contents, string mimeType, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(seriesId, fileName);
            return await _fileStorage.StoreImage(filePath, contents, mimeType, cancellationToken);
        }

        private static string GetUniqueFileName(int seriesId, string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"series/{seriesId}/title.{fileNameWithourExtension}";
        }
    }
}
