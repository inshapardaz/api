using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using Paramore.Brighter;
using File = Inshapardaz.Domain.Entities.File;

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
        private readonly IFileStorage _fileStorage;

        public AddFileRequestHandler(IFileRepository fileRepository, IFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public override async Task<AddFileRequest> HandleAsync(AddFileRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var url = await AddImageToFileStore(command.File.FileName, command.File.Contents, cancellationToken);
            command.Response = await _fileRepository.AddFile(command.File, url, true, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }

        private async Task<string> AddImageToFileStore(string fileName, byte[] contents, CancellationToken cancellationToken)
        {
            var filePath = GetUniqueFileName(fileName);
            return await _fileStorage.StoreFile(filePath, contents, cancellationToken);
        }

        private static string GetUniqueFileName(string fileName)
        {
            var fileNameWithourExtension = Path.GetExtension(fileName).Trim('.');
            return $"images/{Guid.NewGuid():N}.{fileNameWithourExtension}";
        }
    }
}