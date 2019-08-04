using System;

namespace Inshapardaz.Domain.Ports.Library
{
    public abstract class BookRequest : RequestBase
    {
        protected BookRequest(int bookId)
        {
            BookId = bookId;
        }

        public Guid UserId { get; set; }


        public int BookId { get; set; }
    }
}
