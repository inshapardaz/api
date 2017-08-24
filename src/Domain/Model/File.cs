using System;

namespace Inshapardaz.Domain.Model
{
    public class File
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LiveUntil { get; set; }

        public byte[] Contents { get; set; }
    }
}