using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports
{
    public class AddFileRequest : RequestBase
    {
        public AddFileRequest(File file)
        {
            File = file;
        }

        public File File { get; set; }
        public File Response { get; set; }
    }
    public class AddFileRequestHandler : RequestHandlerAsync<AddFileRequest>
    {
        private readonly IFileRepository _fileRepository;

        public AddFileRequestHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public override async Task<AddFileRequest> HandleAsync(AddFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            command.Response = await _fileRepository.AddFile(command.File, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}