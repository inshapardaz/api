using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class PeriodicalDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public int LibraryId { get; set; }

        public string Language { get; set; }
        public PeriodicalFrequency Frequency { get; set; }
    }
}
