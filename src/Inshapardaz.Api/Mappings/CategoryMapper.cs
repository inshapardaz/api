using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Mappings;

public static class CategoryMapper
{
    public static CategoryView Map(this CategoryModel source)
        => new CategoryView
        {
            Id = source.Id,
            Name = source.Name,
            ParentCategoryId = source.ParentCategoryId,
            ParentCategoryName = source.ParentCategoryName,
            BookCount = source.BookCount,
            ArticleCount = source.ArticleCount,
            PoetryCount = source.PoetryCount,
            PeriodicalCount = source.PeriodicalCount,
            ChildCount = source.ChildCount,
            Children = source.Children != null && source.Children.Any()
                ? source.Children.Select(c => c.Map()).ToList()
                : null
        };

    public static CategoryModel Map(this CategoryView source)
        => new CategoryModel
        {
            Id = source.Id,
            Name = source?.Name,
            ParentCategoryId = source?.ParentCategoryId
        };
}
