using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using SixLabors.ImageSharp;
using File = Inshapardaz.Domain.Entities.File;

namespace Inshapardaz.Domain.Ports
{
    public class GetFileRequest : RequestBase
    {
        public GetFileRequest(int imageId, int height, int width)
        {
            ImageId = imageId;
            Height = height;
            Width = width;
        }

        public int ImageId { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        public File Response { get; set; }
    }

    public class GetFileRequestHandler : RequestHandlerAsync<GetFileRequest>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public GetFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<GetFileRequest> HandleAsync(GetFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Response = await _fileRepository.GetFileById(command.ImageId, cancellationToken);

            if (string.IsNullOrWhiteSpace(command.Response.FilePath))
            {
                throw new NotFoundException();
            }

            var contents = await _fileStorage.GetFile(command.Response.FilePath, cancellationToken);
            using (var stream = new MemoryStream(contents))
            using (var output = new MemoryStream())
            {
                using (Image<Rgba32> image = Image.Load(stream))
                {
                    image.Mutate(x => x.Resize(command.Width, command.Height));
                    image.Save(output, ImageFormats.Jpeg);
                    command.Response.Contents = output.GetBuffer();
                }
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
