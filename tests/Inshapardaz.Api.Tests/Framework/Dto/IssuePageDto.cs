﻿using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class IssuePageDto
    {
        public IssuePageDto()
        {
        }

        public IssuePageDto(IssuePageDto source)
        {
            Id = source.Id;
            IssueId = source.IssueId;
            Text = source.Text;
            SequenceNumber = source.SequenceNumber;
            ImageId = source.ImageId;
            Status = source.Status;
            WriterAccountId = source.WriterAccountId;
            ReviewerAccountId = source.ReviewerAccountId;
        }

        public long Id { get; set; }

        public int IssueId { get; set; }

        public string Text { get; set; } 
        public int SequenceNumber { get; set; }

        public long? ImageId { get; set; }
        public long? FileId { get; set; }

        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }

        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }

    }
}
