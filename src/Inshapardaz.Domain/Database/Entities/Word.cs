using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Domain.Database.Entities
{
    public class Word
    {
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string TitleWithMovements { get; set; }

        public string Description { get; set; }

        public GrammaticalType Attributes { get; set; }

        public Languages Language { get; set; }

        public string Pronunciation { get; set; }

        [Required]
        public int DictionaryId { get; set; }

        public virtual Dictionary Dictionary { get; set; }

        public virtual ICollection<Meaning> Meaning { get; set; } = new HashSet<Meaning>();

        public virtual ICollection<Translation> Translation { get; set; } = new HashSet<Translation>();

        public virtual ICollection<WordRelation> WordRelationRelatedWord { get; set; } = new HashSet<WordRelation>();

        public virtual ICollection<WordRelation> WordRelationSourceWord { get; set; } = new HashSet<WordRelation>();
    }
}