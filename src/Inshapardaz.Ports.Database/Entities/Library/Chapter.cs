using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Chapter
    {
        public int Id { get; set; }

        public int ChapterNumber { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public virtual ICollection<ChapterContent> Contents { get; set; }
    }
}