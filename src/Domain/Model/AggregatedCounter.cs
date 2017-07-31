using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class AggregatedCounter
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}