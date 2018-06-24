using System.Collections.Generic;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api.View.Library
{
    public class ChapterView
    {
        public int Id { get; set; }

        public uint ChapterNumber { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int BookId { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
