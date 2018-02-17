using System.Collections.Generic;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordMeaningsByContextQuery : IQuery<IEnumerable<Meaning>>
    {
        public GetWordMeaningsByContextQuery(int dictionaryId, long wordId, string context)
        {
            DictionaryId = dictionaryId;
            WordId = wordId;
            Context = context;
        }

        public int DictionaryId { get; }

        public long WordId { get; }

        public string Context { get; }
    }
}