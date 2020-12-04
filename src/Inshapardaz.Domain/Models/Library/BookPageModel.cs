using System;

namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageModel
    {
        public string Text { get; set; }
        public int SequenceNumber { get; set; }
        public int BookId { get; set; }
        public int? ImageId { get; set; }
        public PageStatuses Status { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? AssignTimeStamp { get; set; }
    }
}
