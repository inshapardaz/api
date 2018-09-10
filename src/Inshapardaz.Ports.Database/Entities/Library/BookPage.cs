using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class BookPage
    {
        public long Id { get; set; }

        [Required]
        public int BookId { get; set; }

        public int? PageNumber { get; set; }

        public virtual Book Book { get; set; }

        public string Text { get; set; }

        public byte[] Contents { get; set; }
    }
}
