using System.Collections.Generic;

namespace Inshapardaz.Api.View.Dictionary
{
    public class RelationshipsView
    {
        public string Title { get; set; }

        public IEnumerable<LinkView> Link { get; set; }

        public IEnumerable<RelationshipView> Relationships { get; set; }
    }
}