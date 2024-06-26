﻿using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class ChapterDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int BookId { get; set; }

        public int ChapterNumber { get; set; }
        public EditingStatus Status { get; set; }

        public int? WriterAccountId { get; set; }

        public DateTime? WriterAssignTimeStamp { get; set; }

        public int? ReviewerAccountId { get; set; }

        public DateTime? ReviewerAssignTimeStamp { get; set; }

    }

    public class ChapterContentDto
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }

        public string Language { get; set; }

        public int FileId { get; set; }
    }
}
