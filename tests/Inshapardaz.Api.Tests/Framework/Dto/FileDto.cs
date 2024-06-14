using System;

namespace Inshapardaz.Api.Tests.Framework.Dto
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
