using System.Collections.Generic;

namespace Inshapardaz.Domain.Models.Library
{
    public class PeriodicalModel
    {
        public int Id { get; set; }
        public int LibraryId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }

        public int IssueCount { get; set; }

        public PeriodicalFrequency Frequency { get; set; }

        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

    }
}
