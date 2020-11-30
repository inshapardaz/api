using Inshapardaz.Domain.Models.Handlers.Library;
using System;
using System.Security.Claims;

namespace Inshapardaz.Domain.Models.Library
{
    public abstract class BookRequest : LibraryAuthorisedCommand
    {
        protected BookRequest(ClaimsPrincipal claims, int libraryId, int bookId, int? userId)
            : base(claims, libraryId)
        {
            BookId = bookId;
            UserId = userId;
        }

        public int BookId { get; set; }
    }
}
