using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Genere> Generes { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        [Required]
        public Languages Language { get; set; }

        public bool IsPublic { get; set; }
    }
}