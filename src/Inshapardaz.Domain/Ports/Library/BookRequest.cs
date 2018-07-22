using System;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports
{
    public abstract class BookRequest : RequestBase
    {
        protected BookRequest(int bookId)
        {
            BookId = bookId;
        }

        public int BookId { get; set; }
    }
}
