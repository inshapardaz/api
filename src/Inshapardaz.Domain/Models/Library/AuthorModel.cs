namespace Inshapardaz.Domain.Models.Library
{
    public class AuthorModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ImageId { get; set; }

        public int BookCount { get; set; }
    }
}
