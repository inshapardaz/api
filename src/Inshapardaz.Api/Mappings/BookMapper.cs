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
        PageCount = source.PageCount,
        ChapterCount = source.ChapterCount,
        PageStatus = source.PageStatus?.Select(ps => ps.Map()),
        Authors = source.Authors?.Select(c => c.Map()).ToList() ?? new System.Collections.Generic.List<AuthorModel>(),
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
        };

    public static BookContentModel Map(this BookContentView source)
        => new BookContentModel
        {
            Id = source.Id,
            BookId = source.BookId,
            Language = source.Language,
            MimeType = source.MimeType
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
}
