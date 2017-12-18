using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;
using System.Collections.Generic;

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