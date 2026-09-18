namespace Inshapardaz.Domain.Models.Library;

public class RatingModel
{
    public long Id { get; set; }

    public int LibraryId { get; set; }

    public int AccountId { get; set; }

    public int Value { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
