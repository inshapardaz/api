using System.Collections.Generic;

namespace Inshapardaz.Api.View
{
    public class JobStatusModel
    {
        public string Status { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}