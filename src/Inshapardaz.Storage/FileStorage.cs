using Inshapardaz.Domain;
using Inshapardaz.Domain.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Storage
{
    public class FileStorage : IFileStorage
    {
        private readonly Settings _settings;

        public FileStorage(Settings settings)
        {
            _settings = settings;
        }

        public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            string name = new CloudBlockBlob(new Uri(filePath)).Name;
            var blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.DeleteAsync(cancellationToken);
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                await DeleteFile(filePath, cancellationToken);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            string name = new CloudBlockBlob(new Uri(filePath)).Name;
            var blockBlob = container.GetBlockBlobReference(name);

            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream, cancellationToken);

                return stream.GetBuffer();
            }
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(name);
            using (Stream stream = new MemoryStream(content))
            {
                await blockBlob.UploadFromStreamAsync(stream, cancellationToken);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        private CloudBlobContainer GetContainer(string container = "library")
        {
            var storageAccount = CloudStorageAccount.Parse(_settings.FileStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(container);
        }

    }
}
