using Amazon.S3;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Storage.FileSystem
{
    public class FileSystemStorage : IFileStorage
    {
        private readonly string _basePath;

        public S3FileStorage(Settings settings)
        {
            _basePath = settings.BasePath;
        }

        public bool SupportsPublicLink => false;

        private string GetFullPath(string filePath) => Path.Combine(_basePath, filePath);

        public Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            return File.ReadAllBytesAsync(GetFullPath(filePath));
        }

        public Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            return File.ReadAllTextAsync(GetFullPath(filePath));
        }

        public Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            return File.WriteAllBytesAsync(GetFullPath(filePath), content);
        }

        public Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            return File.WriteAllBytesAsync(GetFullPath(filePath), content);
        }

        public Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            return File.WriteAllTextAsync(GetFullPath(filePath), content);
        }

        public Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            File.Delete(GetFullPath(filePath));
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            File.Delete(GetFullPath(filePath));
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                File.Delete(GetFullPath(filePath));
            }
            catch
            {
            }
        }

        public async Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                File.Delete(GetFullPath(filePath));
            }
            catch
            {
            }
        }

        public string GetPublicUrl(string filePath)
        {
            throw new NotSupportedException();
        }
    }
}
