﻿using System.Collections.Generic;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class RelationshipByWordIdQuery : IQuery<IEnumerable<WordRelation>>
    {
        public long WordId { get; set; }
    }
}