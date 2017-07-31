using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class JobQueue
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Queue { get; set; }
        public DateTime? FetchedAt { get; set; }
    }
}