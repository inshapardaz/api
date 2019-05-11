using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class ChapterContent
    {
        [Key]
        public int Id { get; set; }

        public int ChapterId { get; set; }

        public string MimeType { get; set; }

        public string ContentUrl { get; set; }
        
        public Chapter Chapter { get; set; }
    }
}