using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public partial class Dictionary
    {
        public Dictionary()
        {
            Words = new HashSet<Word>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Language { get; set; }

        public bool IsPublic { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<Word> Words { get; set; }
    }
}
