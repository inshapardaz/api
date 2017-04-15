using System.Collections.Generic;

namespace Inshapardaz.Domain.Model
{
    public partial class Word
    {
        public Word()
        {
            WordDetails = new HashSet<WordDetail>();
            WordRelations = new HashSet<WordRelation>();
            WordRelatedTo = new HashSet<WordRelation>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public int? DictionaryId { get; set; }
        public string Title { get; set; }
        public string TitleWithMovements { get; set; }
        public string Pronunciation { get; set; }

        public virtual ICollection<WordDetail> WordDetails { get; set; }
        public virtual ICollection<WordRelation> WordRelations { get; set; }
        public virtual ICollection<WordRelation> WordRelatedTo { get; set; }
        public virtual Dictionary Dictionary { get; set; }
    }
}
