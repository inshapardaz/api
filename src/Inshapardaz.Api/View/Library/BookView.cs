using System.Collections.Generic;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api.View.Library
{
    public class BookView
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<LinkView> Links { get; set; }


        public string Description { get; set; }

        public IEnumerable<GenreView> Generes { get; set; }

        public int Language { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}
