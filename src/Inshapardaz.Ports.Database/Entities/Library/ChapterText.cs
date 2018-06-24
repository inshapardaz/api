using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class ChapterText
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public int ChapterId { get; set; }

        public Chapter Chapter { get; set; }
    }
}