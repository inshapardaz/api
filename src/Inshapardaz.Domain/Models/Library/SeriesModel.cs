namespace Inshapardaz.Domain.Models.Library;

public class SeriesModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long? ImageId { get; set; }

    public string ImageUrl { get; set; }

    public int BookCount { get; set; }
}
