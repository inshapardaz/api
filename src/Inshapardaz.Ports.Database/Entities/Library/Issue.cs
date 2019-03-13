using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class Issue
    {
        public int Id { get; set; }

        public int IssueNumber { get; set; }

        public int VolumeNumber { get; set; }

        public DateTime IssueDate { get; set; }

        public int? ImageId { get; set; }

        public int MagazineId { get; set; }

        public virtual Magazine Magazine { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
