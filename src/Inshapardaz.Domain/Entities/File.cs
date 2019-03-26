﻿using System;

namespace Inshapardaz.Domain.Entities
{
    public class File
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public byte[] Contents { get; set; }

        public string FilePath { get; set; }
    }
}