using System;

namespace Inshapardaz.Domain.Helpers
{
    public interface IUserHelper
    {
        
        bool IsAuthenticated { get;  }

        bool IsAdmin { get;  }

        bool IsContributor { get; }

        bool IsReader { get; }

        Guid GetUserId();
    }
}
