using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Chapter
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Key, Column(Order = 1)]
        public uint ChapterNumber { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public int ContentId { get; set; }


        public ChapterText Content { get; set; }
    }

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