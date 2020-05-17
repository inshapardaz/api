using System;

namespace Inshapardaz.Functions.Views.Library
{
    public class ChapterContentView : ViewWithLinks
    {
        public int Id { get; set; }

        //TODO : Delete this
        public int ChapterId { get; set; }

        [Obsolete]
        public string Contents { get; set; }
    }
}
