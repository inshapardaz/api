using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Adapters.Database.SqlServer.Repositories;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Storage.FileSystem;
using Inshapardaz.Storage.S3;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Text.Json;

namespace Inshapardaz.Api.Helpers
{
    public static class FileStorageFactory
    {
        public static IFileStorage GetFileStore(LibraryConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            switch (configuration.FileStoreType)
            {
                case FileStoreTypes.Database:
                    return new SqlServerDatabaseFileStorage(new SqlServerConnectionProvider(configuration., configuration));
                case FileStoreTypes.AzureBlobStorage:
                    return new AzureFileStorage(configuration.FileStoreSource);
                case FileStoreTypes.S3Storage:
                    var config = JsonSerializer.Deserialize<S3Configuration>(configuration.FileStoreSource);
                    return new S3FileStorage(config);
                case FileStoreTypes.FileSystem:
                    var path = configuration.FileStoreSource ?? $"data/{configuration.LibraryId}";
                    var root = new DirectoryInfo(webHostEnvironment.ContentRootPath).Parent.FullName;
                    return new FileSystemStorage(Path.Combine(root, path.TrimStart('/')));
                case FileStoreTypes.Unknown:
                default:
                    throw new ArgumentOutOfRangeException($"file store type `{configuration.FileStoreType}` not supported");
            }
        }
    }
}
