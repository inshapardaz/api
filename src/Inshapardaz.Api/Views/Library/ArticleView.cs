using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library
{
    public class ArticleView : ViewWithLinks
    {
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsPublic { get; set; }

        public IEnumerable<AuthorView> Authors { get; set; }
        public IEnumerable<CategoryView> Categories { get; set; }

        public string Status { get; set; }

        public int? WriterAccountId { get; set; }

        public string WriterAccountName { get; set; }

        public DateTime? WriterAssignTimeStamp { get; set; }

        public int? ReviewerAccountId { get; set; }

        public string ReviewerAccountName { get; set; }

        public DateTime? ReviewerAssignTimeStamp { get; set; }

        public List<ArticleContentView> Contents { get; set; }
    }
}
