using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

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

        public GetFileRequestHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public override async Task<GetFileRequest> HandleAsync(GetFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Response = await _fileRepository.GetFileById(command.ImageId, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
