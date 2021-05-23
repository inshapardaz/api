using Inshapardaz.Domain.Models;
using Newtonsoft.Json;
using System;

namespace Inshapardaz.Api.Views.Library
{
    public class BookPageView : ViewWithLinks
    {
        public int SequenceNumber { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        public int BookId { get; set; }
        public string Status { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public DateTime? AssignTimeStamp { get; set; }
        public int? ChapterId { get; set; }
        public string ChapterTitle { get; set; }
    }
}
