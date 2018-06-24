namespace Inshapardaz.Domain.Entities.Dictionary
{
    public class DictionaryDownload
    {
        public int Id { get; set; }

        public int DictionaryId { get; set; }

        public string File { get; set; }

        public string MimeType { get; set; }
    }
}