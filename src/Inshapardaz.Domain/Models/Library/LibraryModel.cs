using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Domain.Models.Library
{
    public class LibraryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Language { get; set; }

        public Role Role { get; set; }

        public bool SupportsPeriodicals { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string OwnerEmail { get; set; }

        public bool Public { get; set; }
        public DatabaseTypes? DatabaseConnectionType { get; set; }
        public string DatabaseConnection { get; set; }
        public FileStoreTypes? FileStoreType { get; set; }
        public string FileStoreSource { get; set; }
        public int? ImageId { get; set; }
        public string ImageUrl { get; set; }
    }
}
