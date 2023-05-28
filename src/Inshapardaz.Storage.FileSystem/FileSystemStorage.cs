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

        public FileSystemStorage(string basePath)
        {
            _basePath = basePath;
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

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var path = GetFullPath(name);
            path.CreateIfDirectoryDoesNotExists();
            await File.WriteAllBytesAsync(path, content);
            return path;
        }

        public async Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            var path = GetFullPath(name);
            path.CreateIfDirectoryDoesNotExists();
            await File.WriteAllBytesAsync(path, content);
            return path;
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            var path = GetFullPath(name);
            path.CreateIfDirectoryDoesNotExists();
            await File.WriteAllTextAsync(path, content);
            return path;
        }

        public Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            File.Delete(GetFullPath(filePath));
            return Task.CompletedTask;
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            File.Delete(GetFullPath(filePath));
            return Task.CompletedTask;
        }

        public Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                File.Delete(GetFullPath(filePath));
            }
            catch
            {
            }

            return Task.CompletedTask;
        }

        public Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                File.Delete(GetFullPath(filePath));
            }
            catch
            {
            }

            return Task.CompletedTask;
        }

        public string GetPublicUrl(string filePath)
        {
            throw new NotSupportedException();
        }
    }
}
