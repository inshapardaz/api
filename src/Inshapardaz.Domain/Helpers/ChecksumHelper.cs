using System.Security.Cryptography;
using System.Text;

namespace Inshapardaz.Domain.Helpers;

public static class ChecksumHelper
{
    public static string Compute(byte[] contents) =>
        contents == null ? null : Convert.ToHexString(SHA256.HashData(contents)).ToLowerInvariant();

    public static string Compute(string contents) =>
        contents == null ? null : Compute(Encoding.UTF8.GetBytes(contents));
}
