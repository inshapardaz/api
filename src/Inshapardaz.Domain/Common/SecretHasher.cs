namespace Inshapardaz.Domain.Common;

public static class SecretHasher
{
    public static string GetStringHash(string source)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        return BCrypt.Net.BCrypt.HashPassword(source, salt);
    }

    public static bool Verify(string value, string hash) => BCrypt.Net.BCrypt.Verify(value, hash);
}
