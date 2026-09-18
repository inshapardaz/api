using System.Security.Cryptography;

namespace Inshapardaz.Domain.Helpers;

public static class ChecksumHelper
{
    public static string ComputeChecksum(byte[] contents) =>
        contents == null || contents.Length == 0
            ? null
            : Convert.ToHexString(SHA256.HashData(contents)).ToLowerInvariant();
}
