namespace Inshapardaz.Api.Tests.Framework.Dto;

public class NoteDto
{
    public long Id { get; set; }

    public int BookId { get; set; }

    public int LibraryId { get; set; }

    public int AccountId { get; set; }

    public string ClientId { get; set; }

    public string ChapterId { get; set; }

    public int StartOffset { get; set; }

    public int EndOffset { get; set; }

    public string Text { get; set; }

    public string Comment { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
