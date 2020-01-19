namespace Inshapardaz.Domain.Entities.Library
{
    public class Periodical
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public int? CategoryId { get; set; }
    }
}
