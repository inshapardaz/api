namespace Inshapardaz.Domain.Adapters;


public enum FileStoreTypes
{
    Unknown = 0,
    Database,
    AzureBlobStorage,
    S3Storage,
    FileSystem
}
