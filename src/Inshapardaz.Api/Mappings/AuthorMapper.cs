using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Mappings;

public static class AuthorMapper
{
    public static AuthorView Map(this AuthorModel source)
        => source == null ? null : new AuthorView
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            BookCount = source.BookCount,
            ArticleCount = source.ArticleCount,
            PoetryCount = source.PoetryCount,
            AuthorType = source.AuthorType.ToDescription()
        };

    public static AuthorModel Map(this AuthorView source)
        => source == null ? null : new AuthorModel
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            AuthorType = source.AuthorType.ToEnum<AuthorTypes>(AuthorTypes.Writer)
        };
}
