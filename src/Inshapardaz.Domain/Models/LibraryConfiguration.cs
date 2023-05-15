using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Domain.Models
{
    public class LibraryConfiguration
    {
        public string ConnectionString { get; set; }

        public FileStoreTypes FileStoreType { get; set; }

        public string FileStoreSource { get; set; }
    }
}
