using System.Collections.Generic;

using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionariesByUserQuery : IQuery<IEnumerable<Model.Dictionary>>
    {
        public string UserId { get; set; }
    }
}