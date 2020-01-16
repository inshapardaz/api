namespace Inshapardaz.Ports.Database.Entities.Dictionaries
{
    public class DictionaryDownload
    {
        public int Id { get; set; }

        public int DictionaryId { get; set; }

        public int FileId { get; set; }

        public string MimeType { get; set; }

        public virtual Entities.Dictionaries.Dictionary Dictionary { get; set; }

        public virtual File File { get; set; }
    }
}
