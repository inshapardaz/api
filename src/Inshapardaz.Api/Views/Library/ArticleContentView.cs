namespace Inshapardaz.Api.Views.Library;

public class ArticleContentView : ViewWithLinks
{
    public long Id { get; set; }

    public long ArticleId { get; set; }

    public string Language { get; set; }

    public string Text { get; set; }

    public string Layout { get; set; }

}
