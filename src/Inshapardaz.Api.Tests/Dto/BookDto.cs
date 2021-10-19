using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string Language { get; set; }
        public BookStatuses Status { get; set; }

        public int LibraryId { get; set; }

        public int? ImageId { get; set; }

        public int? SeriesId { get; set; }

        public int? SeriesIndex { get; set; }

        public int? YearPublished { get; set; }

        public bool IsPublished { get; set; }

        public CopyrightStatuses Copyrights { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
