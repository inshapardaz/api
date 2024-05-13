namespace Inshapardaz.Api.Views.Library;

public class CategoryView : ViewWithLinks
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int BookCount { get; set; }
}
