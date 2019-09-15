using System;

namespace Inshapardaz.Domain.Ports.Library
{
    public abstract class BookRequest : RequestBase
    {
        protected BookRequest(int bookId, Guid userId)
        {
            BookId = bookId;
            UserId = userId;
        }

        public Guid UserId { get; set; }


        public int BookId { get; set; }
    }
}
