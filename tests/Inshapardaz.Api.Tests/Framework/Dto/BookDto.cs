﻿using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string Language { get; set; }
        public StatusType Status { get; set; }

        public int LibraryId { get; set; }

        public long? ImageId { get; set; }

        public int? SeriesId { get; set; }

        public int? SeriesIndex { get; set; }

        public int? YearPublished { get; set; }

        public bool IsPublished { get; set; }

        public CopyrightStatuses Copyrights { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdated { get; set; }
        public string Source { get; internal set; }
        public string Publisher { get; internal set; }
    }
}
