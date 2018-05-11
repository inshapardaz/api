using System;
using Inshapardaz.Domain.Entities;
using Nest;

namespace Inshapardaz.Ports.Elasticsearch.Entities
{
    [ElasticsearchType(Name = "dictionary")]
    public class Dictionary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        [StringEnum]
        public Languages Language { get; set; }

        public Guid UserId { get; set; }
    }
}