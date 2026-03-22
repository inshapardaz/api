using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

public class PublishBookRequestView(string outputType)
{
    [Required]
    public string OutputType { get; set; } = outputType;

    public bool OnlyPublishFile { get; set; }
}
