namespace Inshapardaz.Domain.Models.Library;

public class BookContentModel
{
    public long Id { get; set; }

    public int BookId { get; set; }

    public string ContentUrl { get; set; }

    public string MimeType { get; set; }

    public string Language { get; set; }

    public string FileName { get; set; }

    public long FileId { get; internal set; }
}
