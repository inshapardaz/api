namespace Inshapardaz.Domain.Exception;

public class UnauthorizedException(string scheme = "Bearer") : System.Exception
{
    public string AuthenticationScheme { get; private set; } = scheme;
}
