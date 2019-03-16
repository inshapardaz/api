using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class BookView
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<LinkView> Links { get; set; }


        public string Description { get; set; }

        public IEnumerable<CategoryView> Categories { get; set; }

        public int Language { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public int? SeriesId { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public DateTime DateAdded {get; set;}

        public DateTime DateUpdated { get; set;}
    }
}
