using System;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class DictionariesWordCountQuery : IQuery<int>
    {
        public int DictionaryId { get; set; }
    }
}