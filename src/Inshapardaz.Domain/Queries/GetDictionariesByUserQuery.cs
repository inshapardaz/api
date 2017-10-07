using System;
using System.Collections.Generic;

using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionariesByUserQuery : IQuery<IEnumerable<Dictionary>>
    {
        public Guid UserId { get; set; }
    }
}