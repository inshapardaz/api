namespace Inshapardaz.Domain.Models.Dictionaries
{
    public class WordRelation
    {
        public long Id { get; set; }
        public long RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }

        public long SourceWordId { get; set; }
        public virtual WordModel RelatedWord { get; set; }
        public virtual WordModel SourceWord { get; set; }
    }
}
