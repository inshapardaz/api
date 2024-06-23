using System;
using System.Security.Cryptography;

namespace Inshapardaz.Domain.Common;

public static class RandomGenerator
{
    public static string GenerateRandomString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(40);
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }
}
