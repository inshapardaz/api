using System.Collections.Generic;

using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionariesByUserQuery : IQuery<IEnumerable<Dictionary>>
    {
        public string UserId { get; set; }
    }
}