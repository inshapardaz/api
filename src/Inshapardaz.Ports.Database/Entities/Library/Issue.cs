using System;
using System.Collections.Generic;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Issue
    {
        public int Id { get; set; }

        public int IssueNumber { get; set; }

        public int VolumeNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public int? ImageId { get; set; }

        public int PeriodicalId { get; set; }

        public virtual Periodical Periodical { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
