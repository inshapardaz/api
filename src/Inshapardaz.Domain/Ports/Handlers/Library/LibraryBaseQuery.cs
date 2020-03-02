using Paramore.Darker;
using System;

namespace Inshapardaz.Domain.Ports.Handlers.Library
{
    public abstract class LibraryBaseQuery<T> : IQuery<T>
    {
        public LibraryBaseQuery(int libraryId)
        {
            LibraryId = libraryId;
        }

        public int LibraryId { get; private set; }
    }

    public abstract class LibraryAuthorisedQuery<T> : LibraryBaseQuery<T>
    {
        public LibraryAuthorisedQuery(int libraryId)
            : base(libraryId)
        {
        }

        public LibraryAuthorisedQuery(int libraryId, Guid userId)
            : base(libraryId)
        {
            UserId = userId;
        }

        public Guid UserId { get; set; }
    }
}
