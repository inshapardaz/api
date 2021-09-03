using System;
using System.Security.Cryptography;

namespace Inshapardaz.Domain.Common
{
    internal static class RandomGenerator
    {
        internal static string GenerateRandomString()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[40];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return BitConverter.ToString(randomBytes).Replace("-", "");
            }
        }
    }
}
