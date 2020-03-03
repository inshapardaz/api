using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories;
using Paramore.Darker;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileModel = Inshapardaz.Domain.Models.FileModel;

namespace Inshapardaz.Domain.Ports
{
    public class GetFileQuery : IQuery<FileModel>
    {
        public GetFileQuery(int imageId, int height, int width)
        {
            ImageId = imageId;
            Height = height;
            Width = width;
        }

        public int ImageId { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public bool IsPublic { get; set; }
    }

    public class GetFileRequestHandler : QueryHandlerAsync<GetFileQuery, FileModel>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorage _fileStorage;

        public GetFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<FileModel> ExecuteAsync(GetFileQuery query, CancellationToken cancellationToken = new CancellationToken())
        {
            var file = await _fileRepository.GetFileById(query.ImageId, cancellationToken);

            if (string.IsNullOrWhiteSpace(file.FilePath))
            {
                throw new NotFoundException();
            }

            var contents = await _fileStorage.GetFile(file.FilePath, cancellationToken);
            using (var stream = new MemoryStream(contents))
            // TODO : Implementation needed
            /*using (var output = new MemoryStream())
            {
                if (IsImageFile(command.Response.MimeType))
                {
                    using (Image<Rgba32> image = Image.Load(stream))
                    {
                        image.Mutate(x => x.Resize(command.Width, command.Height));
                        image.Save(output, ImageFormats.Jpeg);
                        command.Response.Contents = output.GetBuffer();
                    }
                }
                else
                {
                    command.Response.Contents = stream.ToArray();
                }
            }*/
            {
                file.Contents = stream.ToArray();
            }

            return file;
        }

        private bool IsImageFile(string mimeType)
        {
            switch (mimeType.ToLower())
            {
                case "image/bmp":
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                case "image/gif":
                    return true;

                default:
                    return false;
            }
        }
    }
}
