using System;

namespace Inshapardaz.Api.View.Library
{
    public class MagazineIssue : LinkBasedView
    {
        public int IssueNumber { get; set; }

        public int VolumeNumber { get; set; }

        public DateTime IssueDate { get; set; }
    }
}
