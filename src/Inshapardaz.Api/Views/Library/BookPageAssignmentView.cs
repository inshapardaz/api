using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Views.Library
{
    public class BookPageAssignmentView : ViewWithLinks
    {
        public PageStatuses Status { get; set; }
        public int? AccountId { get; set; }
    }
}
