using System;
using Paramore.Brighter;

namespace Inshapardaz.Api.Adapters.Dictionary
{
    public class DictionaryRequest : IRequest
    {
        public Guid Id { get; set; }

        public int DictionaryId { get; set; }
    }
}
