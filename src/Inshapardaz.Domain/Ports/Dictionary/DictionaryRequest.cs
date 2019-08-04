using System;
using System.Security.Claims;

namespace Inshapardaz.Domain.Ports.Dictionary
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
