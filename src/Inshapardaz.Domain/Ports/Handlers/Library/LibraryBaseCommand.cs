using System;
using System.Security.Claims;

namespace Inshapardaz.Domain.Models.Handlers.Library
{
    public abstract class LibraryBaseCommand : RequestBase
    {
        public LibraryBaseCommand(int libraryId)
        {
            LibraryId = libraryId;
        }

        public int LibraryId { get; private set; }
    }

    public abstract class LibraryAuthorisedCommand : AuthoriseRequestBase
    {
        public LibraryAuthorisedCommand(ClaimsPrincipal claims, int libraryId)
            : base(claims)
        {
            LibraryId = libraryId;
        }

        public int LibraryId { get; private set; }

        public Guid? UserId { get; set; }
    }
}
