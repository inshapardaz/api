namespace Inshapardaz.Domain.Adapters.Configuration
{
    public record Storage
    {
        public FileStoreTypes? FileStoreType { get; init; } = FileStoreTypes.Database;
    }
}
