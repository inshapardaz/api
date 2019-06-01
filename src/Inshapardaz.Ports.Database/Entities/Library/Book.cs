using System;
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

        public bool IsPublic { get; set; }

        public BookStatuses Status { get; set; }

        public ICollection<BookCategory> BookCategory { get; set; } = new List<BookCategory>();

        public virtual ICollection<Chapter> Chapters { get; set; }

        public virtual ICollection<BookPage> Pages { get; set; }

        public virtual ICollection<BookFile> Files { get; set; }


        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public int? ImageId { get; set; }

        public int? SeriesId { get; set; }

        public virtual Series Series { get; set; }

        public int? SeriesIndex { get; set; }

        public int? YearPublished { get; set; }

        public bool IsPublished { get; set; }

        public CopyrightStatuses Copyrights { get; set; }

        [Required]
        public Languages Language { get; set; }

        public DateTime DateAdded {get; set;}
        
        public DateTime DateUpdated {get; set;}
    }
}