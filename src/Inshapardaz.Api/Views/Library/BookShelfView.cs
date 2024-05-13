using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

public class BookShelfView : ViewWithLinks
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsPublic { get; set; }

    public int BookCount { get; set; }

}
