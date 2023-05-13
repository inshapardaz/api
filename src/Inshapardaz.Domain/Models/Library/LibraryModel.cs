namespace Inshapardaz.Domain.Models.Library
{
    public class LibraryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public Role Role { get; set; }

        public bool SupportsPeriodicals { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string OwnerEmail { get; set; }

        public bool Public { get; set; }
        public string DatabaseConnection { get; set; }
        public string FileStoreType { get; set; }
        public string FileStoreSource { get; set; }
    }
}
