using System.Collections.Generic;

namespace Inshapardaz.Functions.Views.Library
{
    public class BookFileView : ViewWithLinks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string MimeType { get; set; }
    }

    public class BookFilesView : ViewWithLinks
    {
        public IEnumerable<FileView> Items { get; set; }
    }
}
