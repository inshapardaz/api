using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Model
{
    public class ProcessingStatusView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public string Status { get; set; }
    }
}