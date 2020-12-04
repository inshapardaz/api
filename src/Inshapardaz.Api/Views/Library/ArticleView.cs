using System.Collections.Generic;

namespace Inshapardaz.Api.Views.Library
{
    public class ArticleView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public int AuthorId { get; set; }
        public List<ArticleContentView> Contents { get; internal set; }
    }
}
