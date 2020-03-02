namespace Inshapardaz.Domain.Exception
{
    public class UnauthorizedException : System.Exception
    {
    }

    public class ForbiddenException : System.Exception
    {
        public ForbiddenException(string scheme = "Bearer")
        {
            AuthenticationScheme = scheme;
        }

        public string AuthenticationScheme { get; private set; }
    }
}
