using System.Collections.Generic;

namespace Inshapardaz.Domain.Database.Entities
{
    public class Word
    {
        public Word()
        {
            WordDetail = new HashSet<WordDetail>();
            WordRelationRelatedWord = new HashSet<WordRelation>();
            WordRelationSourceWord = new HashSet<WordRelation>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string TitleWithMovements { get; set; }
        public string Description { get; set; }
        public string Pronunciation { get; set; }
        public int DictionaryId { get; set; }

        public virtual ICollection<WordDetail> WordDetail { get; set; }
        public virtual ICollection<WordRelation> WordRelationRelatedWord { get; set; }
        public virtual ICollection<WordRelation> WordRelationSourceWord { get; set; }
        public virtual Dictionary Dictionary { get; set; }
    }
}