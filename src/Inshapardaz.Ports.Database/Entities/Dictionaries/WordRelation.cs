using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Ports.Database.Entities.Dictionaries
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
