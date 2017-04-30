namespace Inshapardaz.Api.Helpers
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
