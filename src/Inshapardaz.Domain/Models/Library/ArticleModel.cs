using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{

    public class ArticleModel
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public List<AuthorModel> Authors { get; set; } = new List<AuthorModel>();
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public List<ArticleContentModel> Contents { get; set; } = new List<ArticleContentModel>();
        public bool IsRead { get; set; }
        public bool IsFavorite { get; set; }
        public int? ImageId { get; set; }

        public ArticleType Type { get; set; }
        public int? WriterAccountId { get; set; }
        public string WriterAccountName { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public string ReviewerAccountName { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public EditingStatus Status { get; set; }
        public ArticleModel PreviousArticle { get; set; }
        public ArticleModel NextArticle { get; set; }
        public int SourceType { get; set; }
        public int SourceId { get; set; }
        public DateTime? LastModified { get; set; }

    }
}
