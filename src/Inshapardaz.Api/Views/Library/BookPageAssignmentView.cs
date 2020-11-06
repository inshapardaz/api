using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Views.Library
{
    public class BookPageAssignmentView : ViewWithLinks
    {
        public PageStatuses Status { get; set; }
        public Guid? UserId { get; set; }
    }
}
