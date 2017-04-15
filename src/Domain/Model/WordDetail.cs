using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public partial class WordDetail
    {
        public WordDetail()
        {
            Meanings = new HashSet<Meaning>();
            Translations = new HashSet<Translation>();
        }

        public long Id { get; set; }
        public GrammaticalType Attributes { get; set; }
        public Languages Language { get; set; }
        public long WordInstanceId { get; set; }

        public virtual ICollection<Meaning> Meanings { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
        public virtual Word WordInstance { get; set; }
    }
}
