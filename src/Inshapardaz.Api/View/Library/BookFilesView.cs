using System;
using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class BookFilesView : LinkBasedView
    {
        public int BookId { get; set; }

        public IEnumerable<BookFileView> Files { get; set; }
    }
}
