using Paramore.Darker;
using System.Collections.Generic;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordsByTitlesQuery : IQuery<IEnumerable<Word>>
    {
        public GetWordsByTitlesQuery(int dictionaryId, IEnumerable<string> titles)
        {
            DictionaryId = dictionaryId;
            Titles = titles;
        }

        public int DictionaryId { get; }
        public IEnumerable<string> Titles { get; }
    }
}