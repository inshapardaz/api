using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Storage.Azure;

public class AzureFileStorage(string storageConnectionString) : IFileStorage
{
    public bool SupportsPublicLink => true;

    public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        string name = GetBlobName(filePath);
        var blobClient = container.GetBlobClient(name);
        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            await DeleteFile(filePath, cancellationToken);
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
    {
        var container = GetContainer(GetContainerName(filePath));
        string name = GetBlobName(filePath);
        var blobClient = container.GetBlobClient(name);

        using (var stream = new MemoryStream())
        {
            await blobClient.DownloadToAsync(stream, cancellationToken);
            return stream.ToArray();
        }
    }

    public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        string name = GetBlobName(filePath);
        var blobClient = container.GetBlobClient(name);

        BlobDownloadResult result = await blobClient.DownloadContentAsync(cancellationToken);
        return result.Content.ToString();
    }

    public async Task<string> StoreFile(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        var blobClient = container.GetBlobClient(name);
        using (Stream stream = new MemoryStream(content))
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = mimeType }
            }, cancellationToken);
        }

        return blobClient.Uri.AbsolutePath;
    }

    public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        var blobClient = container.GetBlobClient(name);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "text/plain; charset=utf-8" }
            }, cancellationToken);
        }

        return blobClient.Uri.AbsolutePath;
    }

    private BlobContainerClient GetContainer(string container = "library")
    {
        var serviceClient = new BlobServiceClient(storageConnectionString);
        return serviceClient.GetBlobContainerClient(container);
    }

    private string GetContainerName(string url) => new Uri(url).Segments[1].Trim('/');

    private string GetBlobName(string filePath)
    {
        var uri = new Uri(filePath);
        // Skip the first two segments (/ and container name) to get the blob name
        var segments = uri.Segments;
        if (segments.Length > 2)
        {
            return string.Join("", segments, 2, segments.Length - 2).TrimStart('/');
        }
        return segments.Length > 1 ? segments[^1].TrimStart('/') : filePath;
    }

    public async Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
    {
        var container = GetContainer("images");
        var blobClient = container.GetBlobClient(name);
        using (Stream stream = new MemoryStream(content))
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = mimeType }
            }, cancellationToken);
        }

        return blobClient.Uri.AbsolutePath;
    }

    public async Task DeleteImage(string filePath, CancellationToken cancellationToken)
    {
        var container = GetContainer("images");
        string name = GetBlobName(filePath);
        var blobClient = container.GetBlobClient(name);
        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task TryDeleteImage(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            await DeleteImage(filePath, cancellationToken);
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine(e);
        }
    }

    public string GetPublicUrl(string filePath) => filePath;
}
