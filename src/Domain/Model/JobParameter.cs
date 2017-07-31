using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public class JobParameter
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual Job Job { get; set; }
    }
}