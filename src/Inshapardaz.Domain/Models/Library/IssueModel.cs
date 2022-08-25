using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class IssueModel
    {
        public int Id { get; set; }

        public int IssueNumber { get; set; }

        public int VolumeNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public int? ImageId { get; set; }

        public int PeriodicalId { get; set; }

        public virtual PeriodicalModel Periodical { get; set; }

        public string ImageUrl { get; set; }

        public bool IsPublic { get; set; }

        public int ArticleCount { get; set; }
        public int PageCount { get; set; }

        public List<AuthorModel> Authors { get; set; } = new List<AuthorModel>();

        public List<IssueContentModel> Contents { get; set; } = new List<IssueContentModel>();
    }
}
