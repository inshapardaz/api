using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Views.Library
{
    public class BookPageView : ViewWithLinks
    {
        public int SequenceNumber { get; set; }

        public string Text { get; set; }
        public int BookId { get; set; }
        public PageStatuses Status { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? AssignTimeStamp { get; set; }
    }
}
