using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class ArticleDto
    {
        public ArticleDto()
        {
        }

        public ArticleDto(ArticleDto source)
        {
            Id = source.Id;
            Title = source.Title;
            Status = source.Status;
            WriterAccountId = source.WriterAccountId;
            WriterAssignTimeStamp = source.WriterAssignTimeStamp;
            ReviewerAccountId = source.ReviewerAccountId;
            ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp;
        }

        public long Id { get; set; }

        public int LibraryId { get; set; }
        public long? ImageId { get; set; }

        public string Title { get; set; }
        public bool IsPublic{ get; set; }

        public ArticleType Type { get; set; }
        public EditingStatus Status { get; set; }
        public int? WriterAccountId { get; set; }
        public DateTime? WriterAssignTimeStamp { get; set; }
        public int? ReviewerAccountId { get; set; }
        public DateTime? ReviewerAssignTimeStamp { get; set; }

        public int SourceType { get; set; }

        public int SourceId { get; set; }

        public DateTime LastModified { get; set; }


    }

    public class ArticleContentDto
    {
        public int Id { get; set; }

        public long ArticleId { get; set; }

        public string Language { get; set; }

        public string Layout { get; set; }

        public long? FileId { get; set; }
    }

    public class RecentArticleDto
    {
        public long ArticleId { get; set; }

        public int AccountId { get; set; }

        public DateTime DateRead { get; set; }

        public int LibraryId { get; set; }
    }

    public class FavoriteArticleDto
    {
        public long ArticleId { get; set; }

        public int AccountId { get; set; }

        public DateTime DateAdded { get; set; }

        public int LibraryId { get; set; }
    }
}
