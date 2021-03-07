using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class BookModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

        public List<BookContentModel> Contents { get; set; } = new List<BookContentModel>();

        public string Language { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public int? ImageId { get; set; }

        public string ImageUrl { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? SeriesId { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public BookStatuses Status { get; set; }

        public int? YearPublished { get; set; }

        public CopyrightStatuses Copyrights { get; set; }

        public bool IsPublished { get; set; }

        public bool IsFavorite { get; set; }

        public int LibraryId { get; set; }

        public decimal Progress { get; set; }

        public int PageCount { get; set; }

        public IEnumerable<PageSummaryModel> PageStatus { get; set; }
    }
}
