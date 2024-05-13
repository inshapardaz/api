using System;

namespace Inshapardaz.Domain.Models
{
    public class FileModel
    {
        public long Id { get; set; }

        public string MimeType { get; set; }

        public bool IsPublic { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public byte[] Contents { get; set; }

        public string FilePath { get; set; }
    }
}
