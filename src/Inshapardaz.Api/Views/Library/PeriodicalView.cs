using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

public class PeriodicalView : ViewWithLinks
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public int IssueCount { get; set; }

    [Required]
    public string Language { get; set; }

    public IEnumerable<CategoryView> Categories { get; set; }

    [Required]
    public string Frequency { get; set; }
}
