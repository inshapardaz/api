namespace Inshapardaz.Domain.Models.Library;

public class BookShelfModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsPublic { get; set; }

    public long? ImageId { get; set; }

    public string ImageUrl { get; set; }

    public int BookCount { get; set; }

    public int AccountId { get; set; }
}

public class BookShelfBook
{
    public int Id { get; set; }
    public int BookShelfId { get; set; }
    public int BookId { get; set; }
    public int Index { get; set; }
}
