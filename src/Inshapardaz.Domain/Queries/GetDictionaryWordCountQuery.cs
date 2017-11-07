using System;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryWordCountQuery : IQuery<int>
    {
        public int DictionaryId { get; set; }
    }
}