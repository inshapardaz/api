using System;

namespace Inshapardaz.Functions
{
    public static class ConfigurationSettings
    {
        public static string Auth0Domain => GetEnvironmentVariable("Authentication.Authority");
        public static string Audience => GetEnvironmentVariable("Authentication.Audience");
        public static string DatabaseConnectionString => GetEnvironmentVariable("DefaultDatabase");

        private static string GetEnvironmentVariable(string name)
        {
            var result = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException($"Missing app setting {name}");
            return result;
        }
    }
}