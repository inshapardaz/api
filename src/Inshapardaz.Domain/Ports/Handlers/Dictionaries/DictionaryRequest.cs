using System;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class DictionaryRequest : RequestBase
    {
        public DictionaryRequest(int dictionaryId)
        {
            DictionaryId = dictionaryId;
        }

        public int DictionaryId { get; }

        public Guid UserId { get; set;  }
    }
}
