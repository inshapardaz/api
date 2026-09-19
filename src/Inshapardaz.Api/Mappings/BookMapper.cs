using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Mappings;

public static class BookMapper
{
    public static BookView Map(this BookModel source) => new()
    {
        Id = source.Id,
        Title = source.Title,
        Description = source.Description,
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
        Tags = source.Tags?.Select(c => c.Map()),
        PageCount = source.PageCount,
        ChapterCount = source.ChapterCount,
        Source = source.Source,
        Publisher = source.Publisher,
        PageStatus = source.PageStatus?.Select(ps => ps.Map()),
        Authors = source.Authors?.Select(c => c.Map()),
        ReadProgress = source.ReadProgress.Map()
    };

    public static BookModel Map(this BookView source) => new BookModel
    {
        Id = source.Id,
        Title = source.Title,
        Description = source.Description,
        IsPublic = source.IsPublic,
        Language = source.Language,
        DateAdded = source.DateAdded,
        DateUpdated = source.DateUpdated,
        SeriesId = source.SeriesId,
        SeriesIndex = source.SeriesIndex,
        Copyrights = source.Copyrights.ToEnum(CopyrightStatuses.Copyright),
        Status = source.Status.ToEnum(StatusType.AvailableForTyping),
        YearPublished = source.YearPublished,
        IsPublished = source.IsPublished,
        Source = source.Source,
        Publisher = source.Publisher,
        Progress = source.Progress,
        Categories = source.Categories?.Select(c => c.Map()).ToList(),
        Tags = source.Tags?.Select(c => c.Map()).ToList(),
        PageCount = source.PageCount,
        ChapterCount = source.ChapterCount,
        PageStatus = source.PageStatus?.Select(ps => ps.Map()),
        Authors = source.Authors?.Select(c => c.Map()).ToList() ?? new List<AuthorModel>(),
        ReadProgress = source.ReadProgress.Map()
    };

    public static BookContentView Map(this BookContentModel source)
        => new BookContentView
        {
            Id = source.Id,
            BookId = source.BookId,
            Language = source.Language,
            MimeType = source.MimeType,
            FileName = source.FileName,
            Checksum = source.Checksum,
        };

    public static BookContentModel Map(this BookContentView source)
        => new BookContentModel
        {
            Id = source.Id,
            BookId = source.BookId,
            Language = source.Language,
            MimeType = source.MimeType,
            Checksum = source.Checksum
        };

    public static PageSummaryView Map(this PageStatusSummaryModel source)
        => new PageSummaryView
        {
            Status = source.Status.ToDescription(),
            Count = source.Count,
            Percentage = source.Percentage
        };

    public static PageStatusSummaryModel Map(this PageSummaryView source)
        => new PageStatusSummaryModel
        {
            Status = source.Status.ToEnum(EditingStatus.Available),
            Count = source.Count,
            Percentage = source.Percentage
        };

    public static ReadProgressView Map(this ReadProgressModel source)
        => source == null ? null : new ReadProgressView
        {
            ProgressType = source.ProgressType.ToDescription(),
            ProgressId = source.ProgressId,
            ProgressValue = source.ProgressValue
        };

    public static ReadProgressModel Map(this ReadProgressView source)
        => source == null ? null : new ReadProgressModel
        {
            ProgressType = source.ProgressType.ToEnum(ProgressType.Unknown),
            ProgressId = source.ProgressId,
            ProgressValue = source.ProgressValue
        };

    public static BookmarkView Map(this BookmarkModel source)
        => source == null ? null : new BookmarkView
        {
            Id = source.ClientId,
            ChapterId = source.ChapterId,
            Position = source.Position,
            Name = source.Name,
            DateAdded = source.DateAdded,
            DateUpdated = source.DateUpdated
        };

    public static NoteView Map(this NoteModel source)
        => source == null ? null : new NoteView
        {
            Id = source.ClientId,
            ChapterId = source.ChapterId,
            StartOffset = source.StartOffset,
            EndOffset = source.EndOffset,
            Text = source.Text,
            Comment = source.Comment,
            DateAdded = source.DateAdded,
            DateUpdated = source.DateUpdated
        };

    public static RatingView Map(this RatingModel source)
        => source == null ? null : new RatingView
        {
            Value = source.Value,
            DateAdded = source.DateAdded,
            DateUpdated = source.DateUpdated
        };

    public static RatingSummaryView Map(this RatingSummaryModel source)
        => source == null ? null : new RatingSummaryView
        {
            AverageRating = source.AverageRating,
            TotalCount = source.TotalCount
        };
}
