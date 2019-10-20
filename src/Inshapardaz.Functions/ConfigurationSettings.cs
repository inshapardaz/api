using System;

namespace Inshapardaz.Functions
{
    public static class ConfigurationSettings
    {
        public static string Auth0Domain => GetEnvironmentVariable("Authentication.Authority");
        public static string Audience => GetEnvironmentVariable("Authentication.Audience");
        public static string DatabaseConnectionString => GetEnvironmentVariable("DefaultDatabase");

        public static string FileStorageConnectionString => GetEnvironmentVariable("FileStorageConnectionString");
        public static Uri ApiRoot => new Uri(GetEnvironmentVariable("API.Root"));

        private static string GetEnvironmentVariable(string name)
        {
            var result = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException($"Missing app setting {name}");
            return result;
        }
    }
}