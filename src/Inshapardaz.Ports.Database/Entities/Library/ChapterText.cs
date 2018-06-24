using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class ChapterText
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        [Required]
        public int ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; }
    }
}