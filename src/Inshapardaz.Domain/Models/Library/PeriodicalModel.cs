namespace Inshapardaz.Domain.Models.Library
{
    public class PeriodicalModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public string ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public int IssueCount { get; set; }
    }
}
