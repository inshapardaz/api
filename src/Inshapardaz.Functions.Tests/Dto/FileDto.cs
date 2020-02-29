using System;

namespace Inshapardaz.Functions.Tests.Dto
{
    public class FileDto
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public string FilePath { get; set; }

        public bool IsPublic { get; set; }
    }
}
