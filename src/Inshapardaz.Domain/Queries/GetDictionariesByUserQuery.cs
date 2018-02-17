using System;
using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionariesByUserQuery : IQuery<IEnumerable<Dictionary>>
    {
        public Guid UserId { get; set; }
    }
}