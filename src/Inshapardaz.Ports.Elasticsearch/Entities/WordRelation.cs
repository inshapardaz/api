using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Ports.Elasticsearch.Entities
{
    public class WordRelation
    {
        public int Id { get; set; }
        public long RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }
    }
}