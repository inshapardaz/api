namespace Inshapardaz.Api.Views.Library;

public class CategoryView : ViewWithLinks
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentCategoryId { get; set; }

    public string ParentCategoryName { get; set; }

    public int BookCount { get; set; }
    public int ArticleCount { get; set; }
    public int PeriodicalCount { get; set; }
    public int PoetryCount { get; set; }
    public int ChildCount { get; set; }

    public List<CategoryView> Children { get; set; }
}
