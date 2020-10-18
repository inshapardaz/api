using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Storage.Azure
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string _storageConnectionString;

        public AzureFileStorage(Settings settings)
        {
            _storageConnectionString = settings.FileStorageConnectionString;
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
            var container = GetContainer(GetContainerName(filePath));
            string name = new CloudBlockBlob(new Uri(filePath)).Name;
            var blockBlob = container.GetBlockBlobReference(name);

            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream, cancellationToken);

                return stream.GetBuffer();
            }
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            string name = new CloudBlockBlob(new Uri(filePath)).Name;
            var blockBlob = container.GetBlockBlobReference(name);

            return await blockBlob.DownloadTextAsync(cancellationToken);
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(name);
            using (Stream stream = new MemoryStream(content))
            {
                await blockBlob.UploadFromStreamAsync(stream, cancellationToken);
            }

            return blockBlob.Uri.AbsolutePath;
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            var container = GetContainer();
            var blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadTextAsync(content, cancellationToken);

            return blockBlob.Uri.AbsolutePath;
        }

        private CloudBlobContainer GetContainer(string container = "library")
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(container);
        }

        private string GetContainerName(string url)
        {
            return new Uri(url).Segments[1].Trim('/');
        }

        public async Task<string> StoreImage(string name, byte[] content, CancellationToken cancellationToken)
        {
            var container = GetContainer("images");
            var blockBlob = container.GetBlockBlobReference(name);
            using (Stream stream = new MemoryStream(content))
            {
                await blockBlob.UploadFromStreamAsync(stream, cancellationToken);
            }

            return blockBlob.Uri.AbsolutePath;
        }

        public async Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            var container = GetContainer("images");
            string name = new CloudBlockBlob(new Uri(filePath)).Name;
            var blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.DeleteAsync(cancellationToken);
        }

        public async Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                await DeleteImage(filePath, cancellationToken);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
