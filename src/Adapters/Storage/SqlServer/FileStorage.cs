using Inshapardaz.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapradaz.Storage.SqlServer
{
    public class FileStorage : IFileStorage
    {
        public bool SupportsPublicLink => false;

        public Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public string GetPublicUrl(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreImage(string name, byte[] content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
