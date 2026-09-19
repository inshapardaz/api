using System.Security.Cryptography;

namespace Inshapardaz.Api.Tests.Framework.Helpers
{
    public static class ChecksumTestHelper
    {
        public static string Compute(byte[] contents) =>
            contents == null || contents.Length == 0
                ? null
                : Convert.ToHexString(SHA256.HashData(contents)).ToLowerInvariant();
    }
}
