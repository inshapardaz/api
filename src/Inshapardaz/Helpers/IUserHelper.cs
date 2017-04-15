using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Helpers
{
    public interface IUserHelper
    {
        
        bool IsAuthenticated { get;  }

        bool IsAdmin { get;  }

        bool IsContributor { get; }

        bool IsReader { get; }

        string GetUserId();
    }
}
