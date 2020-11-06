using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.Diagnostics;
using SixLabors.ImageSharp;
using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class BookPageDto
    {
        public BookPageDto()
        {
        }

        public BookPageDto(BookPageDto source)
        {
            Id = source.Id;
            BookId = source.BookId;
            Text = source.Text;
            SequenceNumber = source.SequenceNumber;
            ImageId = source.ImageId;
            Status = source.Status;
            UserId = source.UserId;
        }

        public long Id { get; set; }

        public int BookId { get; set; }

        public string Text { get; set; }

        public int SequenceNumber { get; set; }

        public int? ImageId { get; set; }

        public PageStatuses Status { get; set; }
        public Guid? UserId { get; set; }
    }
}
