using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library
{
    public class IssueArticleView : ViewWithLinks
    {
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int SequenceNumber { get; set; }

        public string SeriesName { get; set; }

        public int? SeriesIndex { get; set; }

        public IEnumerable<AuthorView> Authors { get; set; }

        public string Status { get; set; }

        public int? WriterAccountId { get; set; }

        public string WriterAccountName { get; set; }

        public DateTime? WriterAssignTimeStamp { get; set; }

        public int? ReviewerAccountId { get; set; }

        public string ReviewerAccountName { get; set; }

        public DateTime? ReviewerAssignTimeStamp { get; set; }

        public List<IssueArticleContentView> Contents { get; internal set; }
    }
}
