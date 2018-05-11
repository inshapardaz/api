using System.Collections.Generic;
namespace Inshapardaz.Domain.Entities
{
    public class Word
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string TitleWithMovements { get; set; }

        public string Description { get; set; }

        public GrammaticalType Attributes { get; set; }

        public Languages Language { get; set; }

        public string Pronunciation { get; set; }

        public int DictionaryId { get; set; }

        public virtual Dictionary Dictionary { get; set; }

        public ICollection<Meaning> Meaning { get; set; } = new List<Meaning>();

        public ICollection<Translation> Translation { get; set; } = new List<Translation>();

        public ICollection<WordRelation> Relations { get; set; } = new List<WordRelation>();

        public virtual ICollection<WordRelation> WordRelationRelatedWord { get; set; } = new HashSet<WordRelation>();

        public virtual ICollection<WordRelation> WordRelationSourceWord { get; set; } = new HashSet<WordRelation>();
    }
}