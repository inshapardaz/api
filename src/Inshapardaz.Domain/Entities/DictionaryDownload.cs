namespace Inshapardaz.Domain.Entities
{
    public class DictionaryDownload
    {
        public int Id { get; set; }

        public int DictionaryId { get; set; }

        public int FileId { get; set; }

        public string MimeType { get; set; }
    }
}