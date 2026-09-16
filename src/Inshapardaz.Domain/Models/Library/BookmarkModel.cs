namespace Inshapardaz.Domain.Models.Library;

public class BookmarkModel
{
    // Internal auto-increment PK - never exposed on BookmarkView, which exposes ClientId as its
    // own Id instead (see that class's own doc comment for why).
    public long Id { get; set; }

    public int BookId { get; set; }

    public int LibraryId { get; set; }

    public int AccountId { get; set; }

    public string ClientId { get; set; }

    public string ChapterId { get; set; }

    public double Position { get; set; }

    public string Name { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
