using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library
{
    public class AuthorView : ViewWithLinks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookCount { get; set; }
        public string AuthorType { get; set; }
        public int ArticleCount { get; set; }
    }
}
