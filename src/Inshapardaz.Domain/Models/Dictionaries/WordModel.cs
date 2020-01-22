using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Dictionaries
{
    public class WordModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string TitleWithMovements { get; set; }

        public string Description { get; set; }

        public GrammaticalType Attributes { get; set; }

        public Languages Language { get; set; }

        public string Pronunciation { get; set; }

        public int DictionaryId { get; set; }

        public virtual DictionaryModel Dictionary { get; set; }

        public ICollection<MeaningModel> Meaning { get; set; } = new List<MeaningModel>();

        public ICollection<Translation> Translation { get; set; } = new List<Translation>();

        public ICollection<WordRelation> Relations { get; set; } = new List<WordRelation>();

        public virtual ICollection<WordRelation> WordRelationRelatedWord { get; set; } = new HashSet<WordRelation>();

        public virtual ICollection<WordRelation> WordRelationSourceWord { get; set; } = new HashSet<WordRelation>();
    }
}
