namespace Inshapardaz.Domain.Database.Entities
{
    public class WordRelation
    {
        public long Id { get; set; }
        public long SourceWordId { get; set; }
        public long RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }

        public virtual Word RelatedWord { get; set; }
        public virtual Word SourceWord { get; set; }
    }
}