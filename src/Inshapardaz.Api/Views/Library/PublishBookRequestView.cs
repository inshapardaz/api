using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

public class PublishBookRequestView
{
    public PublishBookRequestView(string outputType)
    {
        OutputType = outputType;
    }

    [Required]
    public string OutputType { get; set; }

    public bool OnlyPublishFile { get; set; }
}
