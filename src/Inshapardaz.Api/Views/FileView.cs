﻿using System;

namespace Inshapardaz.Api.Views
{
    public class FileView : ViewWithLinks
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
