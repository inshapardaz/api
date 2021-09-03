using BCrypt.Net;

namespace Inshapardaz.Domain.Common
{
    public static class SecretHasher
    {
        //TODO : Add per user salt
        public static string GetStringHash(string source) => BCrypt.Net.BCrypt.HashPassword(source);

        public static bool Verify(string value, string hash) => BCrypt.Net.BCrypt.Verify(value, hash);
    }
}
