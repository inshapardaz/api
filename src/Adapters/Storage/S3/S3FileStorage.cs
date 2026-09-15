using Amazon.S3;
using Amazon.S3.Model;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Storage.S3;


public class S3FileStorage(S3Configuration configuration, ILogger<S3FileStorage> logger) : IFileStorage
{
    public bool SupportsPublicLink => false;

    public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
    {
        var key = $"{configuration.FolderName}/{filePath}";
        try
        {
            var client = GetClient();
            var request = new GetObjectRequest();
            request.Key = key;
            request.BucketName = configuration.BucketName;
            var response = await client.GetObjectAsync(request, cancellationToken);
            return await ReadAllContents(response.ResponseStream);
        }
        catch (AmazonS3Exception ex)
        {
            // Every S3 failure -- object genuinely missing, wrong bucket,
            // bad/expired credentials -- collapses into the same null here,
            // which the caller then reports as an identical opaque 404.
            // Log the real reason so a "download 404s" report is diagnosable
            // without needing direct S3 access.
            logger.LogWarning(ex, "S3 GetFile failed for key {Key} in bucket {Bucket}: {ErrorCode} {StatusCode} {Message}",
                key, configuration.BucketName, ex.ErrorCode, ex.StatusCode, ex.Message);
            return null;
        }
    }

    public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var client = GetClient();
            var request = new GetObjectRequest();
            request.Key = $"{configuration.FolderName}/{filePath}";
            request.BucketName = configuration.BucketName;
            var response = await client.GetObjectAsync(request, cancellationToken);
            return await ReadAllText(response.ResponseStream);
        }
        catch (AmazonS3Exception ex)
        {
            return null;
        }
    }

    public async Task<string> StoreFile(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var request = new PutObjectRequest();
        request.BucketName = configuration.BucketName;
        request.ContentType = mimeType;
        request.InputStream = new MemoryStream(content);
        request.Key = $"{configuration.FolderName}/{name}";
        request.CannedACL = "private";
        var response = await client.PutObjectAsync(request, cancellationToken);
        return name;
    }

    public async Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var request = new PutObjectRequest();
        request.BucketName = configuration.BucketName;
        request.ContentType = mimeType ?? MimeTypes.Text;
        request.InputStream = new MemoryStream(content);
        request.Key = $"{configuration.FolderName}/{name}";
        request.CannedACL = "public-read";
        var response = await client.PutObjectAsync(request, cancellationToken);
        return name;
    }

    public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var request = new PutObjectRequest();
        request.BucketName = configuration.BucketName;
        request.ContentType = MimeTypes.Text;
        request.ContentBody = content;
        request.Key = $"{configuration.FolderName}/{name}";
        request.CannedACL = "private ";
        var response = await client.PutObjectAsync(request, cancellationToken);
        return name;
    }

    public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var request = new DeleteObjectRequest();
        request.BucketName = configuration.BucketName;
        request.Key = $"{configuration.FolderName}/{filePath}";
        await client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task DeleteImage(string filePath, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var request = new DeleteObjectRequest();
        request.BucketName = configuration.BucketName;
        request.Key = $"{configuration.FolderName}/{filePath}";
        await client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
    {
        var client = GetClient();
        try
        {
            var request = new GetObjectMetadataRequest();
            request.BucketName = configuration.BucketName;
            request.Key = $"{configuration.FolderName}/{filePath}";
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
            var request = new DeleteObjectRequest();
            request.BucketName = configuration.BucketName;
            request.Key = $"{configuration.FolderName}/{filePath}";
            await client.DeleteObjectAsync(request, cancellationToken);
        }
        catch
        {
        }
    }

    private AmazonS3Client GetClient()
    {
        AmazonS3Config config = new AmazonS3Config();
        config.ServiceURL = configuration.ServiceUrl;

        return new AmazonS3Client(
                configuration.AccessKey,
                configuration.AccessSecret,
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
        GetPreSignedUrlRequest preSignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = configuration.BucketName,
            Key = $"{configuration.FolderName}/{filePath}",
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        return GetClient().GetPreSignedURL(preSignedUrlRequest);
    }
}
