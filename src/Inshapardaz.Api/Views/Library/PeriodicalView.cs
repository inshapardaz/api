using System.Collections.Generic;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Views.Library
{
    public class PeriodicalView : ViewWithLinks
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int IssueCount { get; set; }

        public string Language { get; set; }

        public IEnumerable<CategoryView> Categories { get; set; }
        public string Frequency { get;  set; }
    }
}
