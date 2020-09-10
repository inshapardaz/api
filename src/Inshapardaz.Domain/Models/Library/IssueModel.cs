using System;

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
    }
}
