using System.Collections.Generic;

namespace Inshapardaz.Domain.Entities.Library
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<Genre> Generes { get; set; }

        public Languages Language { get; set; }

        public bool IsPublic { get; set; }

        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
}