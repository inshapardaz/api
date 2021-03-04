using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using System.Linq;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Mappings
{
    public static class BookMapper
    {
        public static BookView Map(this BookModel source)
            => new BookView
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                AuthorName = source.AuthorName,
                IsPublic = source.IsPublic,
                Language = source.Language,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesName = source.SeriesName,
                SeriesIndex = source.SeriesIndex,
                Copyrights = source.Copyrights.ToDescription(),
                Status = source.Status.ToDescription(),
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished,
                Progress = source.Progress,
                Categories = source.Categories?.Select(c => c.Map()),
                PageStatus = source.PageStatus?.Select(ps => ps.Map())
            };

        public static BookModel Map(this BookView source)
            => new BookModel
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                IsPublic = source.IsPublic,
                Language = source.Language,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesIndex = source.SeriesIndex,
                Copyrights = source.Copyrights.ToEnum(CopyrightStatuses.Copyright),
                Status = source.Status.ToEnum(BookStatuses.AvailableForTyping),
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished,
                Progress = source.Progress,
                Categories = source.Categories?.Select(c => c.Map()).ToList(),
                PageStatus = source.PageStatus?.Select(ps => ps.Map())
            };

        public static BookContentView Map(this BookContentModel source)
            => new BookContentView
            {
                Id = source.Id,
                BookId = source.BookId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static BookContentModel Map(this BookContentView source)
            => new BookContentModel
            {
                Id = source.Id,
                BookId = source.BookId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static PageSummaryView Map(this PageSummaryModel source)
            => new PageSummaryView
            {
                Status = source.Status.ToDescription(),
                Count = source.Count
            };

        public static PageSummaryModel Map(this PageSummaryView source)
            => new PageSummaryModel
            {
                Status = source.Status.ToEnum(PageStatuses.Available),
                Count = source.Count
            };
    }
}
