using System;

namespace Inshapardaz.Domain.Adapters
{
    public static class ConfigurationSettings
    {
        static ConfigurationSettings()
        {
            Auth0Domain = GetEnvironmentVariable("Authentication.Authority");
            Audience = GetEnvironmentVariable("Authentication.Audience");
            DatabaseConnectionString = GetEnvironmentVariable("DefaultDatabase");
            CDNAddress = GetEnvironmentVariable("CDNAddress");
            BlobRoot = GetEnvironmentVariable("BlobRoot");
            FileStorageConnectionString = GetEnvironmentVariable("FileStorageConnectionString");
            ApiRoot = new Uri(GetEnvironmentVariable("API.Root"));
        }

        public static string Auth0Domain { get; set; }
        public static string Audience { get; set; }
        public static string DatabaseConnectionString { get; set; }
        public static string CDNAddress { get; set; }
        public static string BlobRoot { get; set; }
        public static string FileStorageConnectionString { get; set; }
        public static Uri ApiRoot { get; set; }

        private static string GetEnvironmentVariable(string name)
        {
            var result = Environment.GetEnvironmentVariable(name);
            //if (string.IsNullOrEmpty(result))
            //throw new InvalidOperationException($"Missing app setting {name}");
            return result;
        }
    }
}
