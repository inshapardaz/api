using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views;

public class ChughtaiDownloadView
{
    [Required]
    public string BookUrl { get; set; }
    public string SessionId { get; set; }
    public bool ConvertToPdf { get; set; }
}
