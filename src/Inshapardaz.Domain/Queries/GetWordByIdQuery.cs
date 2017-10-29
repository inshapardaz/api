using System;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetWordByIdQuery : IQuery<Word>
    {
        public long WordId { get; set; }
        
        public int DictionaryId { get; set; }
    }
}