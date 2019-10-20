using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Views
{
    public class FileView : ViewWithLinks
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
