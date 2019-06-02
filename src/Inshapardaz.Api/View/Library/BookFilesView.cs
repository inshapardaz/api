using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class BookFilesView
    {
        public int BookId { get; set; }
        public IEnumerable<LinkView> Links { get; set; }

        public IEnumerable<BookFileView> Files { get; set; }
    }
}
