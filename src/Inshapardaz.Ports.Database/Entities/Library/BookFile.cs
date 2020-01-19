using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class BookFile
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int FileId { get; set; }

        public virtual Book Book { get; set; }

        public virtual File File { get; set; }
    }
}
