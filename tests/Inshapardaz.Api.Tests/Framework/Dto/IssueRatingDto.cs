namespace Inshapardaz.Api.Tests.Framework.Dto;

public class IssueRatingDto
{
    public long Id { get; set; }
    public int IssueId { get; set; }
    public int LibraryId { get; set; }
    public int AccountId { get; set; }
    public int Value { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateUpdated { get; set; }
}
