﻿using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Mappings;

public static class ArticleMapper
{
    public static ArticleView Map(this ArticleModel source)
        => source == null ? null : new ArticleView
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            Status = source.Status.ToDescription(),
            Type = source.Type.ToDescription(),
            WriterAccountId = source.WriterAccountId,
            WriterAccountName = source.WriterAccountName,
            WriterAssignTimeStamp = source.WriterAssignTimeStamp,
            ReviewerAccountId = source.ReviewerAccountId,
            ReviewerAccountName = source.ReviewerAccountName,
            ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp,
            Authors = source.Authors?.Select(a => a.Map()),
            Categories = source.Categories?.Select(c => c.Map()),
            Tags = source.Tags?.Select(c => c.Map()),
            Contents = source?.Contents?.Select(c => c.Map()).ToList(),
            LastModified = source?.LastModified
        };

    public static ArticleModel Map(this ArticleView source)
        => source == null ? null : new ArticleModel
        {
            Id = source.Id,
            Title = source.Title,
            IsPublic = source.IsPublic,
            Status = source.Status.ToEnum(EditingStatus.Available),
            Type = source.Type.ToEnum(ArticleType.Unknown),
            WriterAccountId = source.WriterAccountId,
            WriterAccountName = source.WriterAccountName,
            WriterAssignTimeStamp = source.WriterAssignTimeStamp,
            ReviewerAccountId = source.ReviewerAccountId,
            ReviewerAccountName = source.ReviewerAccountName,
            ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp,
            Authors = source.Authors?.Select(x => x.Map()).ToList(),
            Categories = source.Categories?.Select(x => x.Map()).ToList(),
            Tags = source.Tags?.Select(x => x.Map()).ToList(),
            Contents = source?.Contents?.Select(c => c.Map()).ToList(),
            LastModified = source?.LastModified
        };

    public static ArticleContentView Map(this ArticleContentModel source)
        => new ArticleContentView
        {
            Id = source.Id,
            ArticleId = source.ArticleId,
            Language = source.Language,
            Text = source.Text,
            Layout = source.Layout
        };

    public static ArticleContentModel Map(this ArticleContentView source)
        => new ArticleContentModel
        {
            Id = source.Id,
            ArticleId = source.ArticleId,
            Language = source.Language,
            Text = source.Text,
            Layout = source.Layout
        };
}
