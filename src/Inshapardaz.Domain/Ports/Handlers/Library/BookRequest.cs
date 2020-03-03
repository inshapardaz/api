using Inshapardaz.Domain.Ports.Handlers.Library;
using System;
using System.Security.Claims;

namespace Inshapardaz.Domain.Ports.Library
{
    public abstract class BookRequest : LibraryAuthorisedCommand
    {
        protected BookRequest(ClaimsPrincipal claims, int libraryId, int bookId, Guid userId)
            : base(claims, libraryId)
        {
            BookId = bookId;
            UserId = userId;
        }

        public int BookId { get; set; }
    }
}
