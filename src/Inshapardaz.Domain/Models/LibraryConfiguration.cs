using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Models
{
    public class LibraryConfiguration
    {
        public int LibraryId { get; set; }

        public DatabaseTypes? DatabaseConnectionType { get; set; }

        public string ConnectionString { get; set; }

        public FileStoreTypes FileStoreType { get; set; }

        public string FileStoreSource { get; set; }
    }
}
