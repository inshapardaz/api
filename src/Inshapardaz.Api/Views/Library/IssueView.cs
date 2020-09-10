using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Views.Library
{
    public class IssueView : ViewWithLinks
    {
        public int Id { get; set; }

        public int IssueNumber { get; set; }

        public int VolumeNumber { get; set; }

        public DateTime IssueDate { get; set; }
    }
}
