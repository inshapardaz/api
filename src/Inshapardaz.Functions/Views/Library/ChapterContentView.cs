using System;

namespace Inshapardaz.Functions.Views.Library
{
    public class ChapterContentView : ViewWithLinks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int ChapterId { get; set; }

        public string Language { get; set; }

        public string MimeType { get; internal set; }
    }
}
