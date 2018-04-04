using System.Runtime.Serialization;
using Nest;

namespace Inshapardaz.Domain.Entities
{
    public class WordRelation
    {
        public int Id { get; set; }
        public long RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }
    }
}