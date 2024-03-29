﻿using System;

namespace Inshapardaz.Domain.Models.Library
{
    public class BookPageModel
    {
        public string Text { get; set; }
        public int SequenceNumber { get; set; }
        public int BookId { get; set; }
        public int? ImageId { get; set; }
        public string ImageUrl { get; set; }
        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public string WriterAccountName { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public string ReviewerAccountName { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public int? ChapterId { get; set; }

        public string ChapterTitle { get; set; }
        public BookPageModel PreviousPage { get; internal set; }
        public BookPageModel NextPage { get; internal set; }
    }
}
