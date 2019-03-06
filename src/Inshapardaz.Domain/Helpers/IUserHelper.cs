using System;

namespace Inshapardaz.Domain.Helpers
{
    public interface IUserHelper
    {
        
        bool IsAuthenticated { get;  }

        bool IsAdmin { get;  }

        bool IsWriter { get; }

        bool IsReader { get; }

        Guid GetUserId();
    }
}
