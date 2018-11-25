using System.Collections.Generic;

namespace Inshapardaz.Api.View.Library
{
    public class AuthorView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BookCount { get; set; }

        public IEnumerable<LinkView> Links { get; set; }
    }
}
