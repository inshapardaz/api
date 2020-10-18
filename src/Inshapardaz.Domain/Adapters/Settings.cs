using System;

namespace Inshapardaz.Domain.Adapters
{
    public class Settings
    {
        public string DefaultConnection { get; set; }
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string CDNAddress { get; set; }
        public string BlobRoot { get; set; }
        public string FileStorageConnectionString { get; set; }
        public Uri ApiRoot { get; set; }

        public FileStoreTypes FileStoreType { get; set; }
    }
}
