using System;

namespace Inshapardaz.Functions.Authentication
{
    public static class Constants
    {
        public static string Auth0Domain => GetEnvironmentVariable("AUTH_DOMAIN");
        public static string Audience => GetEnvironmentVariable("AUTH_AUDIENCE");

        private static string GetEnvironmentVariable(string name)
        {
            var result = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException($"Missing app setting {name}");
            return result;
        }
    }
}