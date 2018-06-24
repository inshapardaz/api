using System;
using System.Collections.Generic;

namespace Inshapardaz.Domain.Entities.Dictionary
{
    public class Dictionary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public Languages Language { get; set; }

        public Guid UserId { get; set; }

        public virtual IEnumerable<DictionaryDownload> Downloads { get; set; }
    }
}