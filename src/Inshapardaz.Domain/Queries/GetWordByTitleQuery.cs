using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordByTitleQuery : IQuery<Word>
    {
        public GetWordByTitleQuery(int dictionaryId, string title)
        {
            DictionaryId = dictionaryId;
            Title = title;
        }

        public int DictionaryId { get; }
        public string Title { get; }
    }
}