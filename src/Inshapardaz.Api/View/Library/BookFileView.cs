using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class BookFileView
    {
        public IEnumerable<LinkView> Links { get; set; }

        public string MimeType { get; set; }
    }
}
