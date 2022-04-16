using Amazon.S3;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Storage.S3
{
    public class S3FileStorage : IFileStorage
    {
        private readonly string _accessKey, _accessSecret, _serviceUrl, _bucketName;

        public S3FileStorage(Settings settings)
        {
            _serviceUrl = settings.S3ServiceUrl;
            _accessKey = settings.S3Accesskey;
            _accessSecret = settings.S3AccessSecret;
            _bucketName = settings.S3BucketName;
        }

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.GetObjectRequest();
            request.Key = filePath;
            request.BucketName = _bucketName;
            var response = await client.GetObjectAsync(request, cancellationToken);
            return await ReadAllContents(response.ResponseStream);
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.GetObjectRequest();
            request.Key = filePath;
            request.BucketName = _bucketName;
            var response = await client.GetObjectAsync(request, cancellationToken);
            return await ReadAllText(response.ResponseStream);
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.PutObjectRequest();
            request.BucketName = _bucketName;
            request.ContentType = "";
            request.InputStream = new MemoryStream(content);
            request.Key = name;
            request.CannedACL = "private";
            var response = await client.PutObjectAsync(request, cancellationToken);
            return name;
        }

        public async Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.PutObjectRequest();
            request.BucketName = _bucketName;
            request.ContentType = mimeType;
            request.InputStream = new MemoryStream(content);
            request.Key = name;
            request.CannedACL = "public-read";
            var response = await client.PutObjectAsync(request, cancellationToken);
            return name;
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.PutObjectRequest();
            request.BucketName = _bucketName;
            request.ContentType = "";
            request.ContentBody = content;
            request.Key = name;
            request.CannedACL = "private ";
            var response = await client.PutObjectAsync(request, cancellationToken);
            return name;
        }

        public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.DeleteObjectRequest();
            request.BucketName = _bucketName;
            request.Key = filePath;
            await client.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task DeleteImage(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            var request = new Amazon.S3.Model.DeleteObjectRequest();
            request.BucketName = _bucketName;
            request.Key = filePath;
            await client.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            try
            {
                var request = new Amazon.S3.Model.GetObjectMetadataRequest();
                request.BucketName = _bucketName;
                request.Key = filePath;
                await client.GetObjectMetadataAsync(request, cancellationToken);
                await DeleteFile(filePath, cancellationToken);
            }
            catch
            {
            }
        }

        public async Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
        {
            var client = GetClient();
            try
            {
                var request = new Amazon.S3.Model.DeleteObjectRequest();
                request.BucketName = _bucketName;
                request.Key = filePath;
                await client.DeleteObjectAsync(request, cancellationToken);
            }
            catch
            {
            }
        }

        private AmazonS3Client GetClient()
        {
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = _serviceUrl;

            return new AmazonS3Client(
                    _accessKey,
                    _accessSecret,
                    config);
        }

        private async Task<string> ReadAllText(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string contents = await reader.ReadToEndAsync();
                return contents;
            }
        }

        private async Task<byte[]> ReadAllContents(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public string GetPublicUrl(string filePath)
        {
            return new Uri(new Uri(_serviceUrl), $"{_bucketName}/" + filePath).ToString();
        }
    }
}
