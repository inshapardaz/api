namespace Inshapardaz.Domain.Exception;

public class UnauthorizedException : System.Exception
{
    public UnauthorizedException(string scheme = "Bearer")
    {
        AuthenticationScheme = scheme;
    }

    public string AuthenticationScheme { get; private set; }
}
