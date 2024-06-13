namespace Inshapardaz.Domain.Models.Library;

public class ArticleContentModel
{
    public long Id { get; set; }

    public long ArticleId { get; set; }

    public string Text { get; set; }
    public string Language { get; set; }
    public string Layout { get; set; }
    public long? FileId { get; set; }
    public long? ImageId { get; set; }
}
