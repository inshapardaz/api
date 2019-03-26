using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inshapardaz.Api.View
{
    public class FileView
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
