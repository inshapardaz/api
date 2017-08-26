using System.Collections.Generic;

namespace Inshapardaz.Domain.Database.Entities
{
    public class WordDetail
    {
        public WordDetail()
        {
            Meaning = new HashSet<Meaning>();
            Translation = new HashSet<Translation>();
        }

        public long Id { get; set; }
        public GrammaticalType Attributes { get; set; }
        public long WordInstanceId { get; set; }
        public Languages Language { get; set; }

        public virtual ICollection<Meaning> Meaning { get; set; }
        public virtual ICollection<Translation> Translation { get; set; }
        public virtual Word WordInstance { get; set; }
    }
}