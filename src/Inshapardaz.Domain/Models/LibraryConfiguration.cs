using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Domain.Models
{
    public class LibraryConfiguration
    {
        public int LibraryId { get; set; }
        public string ConnectionString { get; set; }

        public FileStoreTypes FileStoreType { get; set; }

        public string FileStoreSource { get; set; }
    }
}
