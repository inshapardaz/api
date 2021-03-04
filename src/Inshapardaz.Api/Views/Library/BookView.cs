using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.Views.Library
{
    public class BookView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<CategoryView> Categories { get; set; }

        public IEnumerable<BookContentView> Contents { get; set; }

        public string Language { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public int? SeriesId { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? YearPublished { get; set; }

        public string Copyrights { get; set; }

        public string Status { get; set; }

        public bool IsPublished { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Progress { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<PageSummaryView> PageStatus { get; set; }
    }
}
