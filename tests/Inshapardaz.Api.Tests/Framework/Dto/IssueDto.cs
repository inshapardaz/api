﻿using System;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class IssueDto
    {
        public int Id { get; set; }

        public int PeriodicalId { get; set; }

        public int VolumeNumber { get; set; }

        public int IssueNumber { get; set; }

        public int LibraryId { get; set; }

        public long? ImageId { get; set; }

        public DateTime IssueDate { get; set; }

        public bool IsPublic { get; set; }

        public string SeriesName { get; set; }

        public int SeriesIndex { get; set; }

        public int? WriterAccountId { get; set; }

        public DateTime? WriterAssignTimeStamp { get; set; }

        public int? ReviewerAccountId { get; set; }

        public DateTime? ReviewerAssignTimeStamp { get; set; }
        public StatusType Status { get; set; }
    }

    public class IssueContentDto
    {
        public int Id { get; set; }

        public int IssueId { get; set; }

        public long FileId { get; set; }

        public string Language { get; set; }

        public string MimeType { get; set; }

        public string FilePath { get; set; }
    }

}
