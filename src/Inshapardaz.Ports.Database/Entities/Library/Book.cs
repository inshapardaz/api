using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public BookStatuses Status { get; set; }

        public ICollection<BookCategory> BookCategory { get; set; } = new List<BookCategory>();

        public virtual ICollection<Chapter> Chapters { get; set; }

        public virtual ICollection<BookPage> Pages { get; set; }


        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public int? ImageId { get; set; }    

        [Required]
        public Languages Language { get; set; }
    }
}