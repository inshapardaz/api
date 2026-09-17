namespace Inshapardaz.Api.Tests.Framework.Dto;

public class BookmarkDto
{
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
