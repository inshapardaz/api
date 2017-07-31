using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class List
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}