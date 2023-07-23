using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Views.Library
{
    public class AssignmentView : ViewWithLinks
    {
        public bool Unassign { get; set; }
    
        public int? AccountId { get; set; }
    }
}
