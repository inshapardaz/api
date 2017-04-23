using System.Collections.Generic;

using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionariesByUserQuery : IQuery<IEnumerable<Model.Dictionary>>
    {
        public string UserId { get; set; }
    }
}