﻿using Paramore.Darker;
using System;

namespace Inshapardaz.Domain.Models.Handlers.Library
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

        public LibraryAuthorisedQuery(int libraryId, int? userId)
            : base(libraryId)
        {
            UserId = userId;
        }

        public int? UserId { get; set; }
    }
}
