using System.ComponentModel.DataAnnotations;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Views.Library;

public class ReadProgressView
{
    public string ProgressType { get; set; }

    public long ProgressId { get; set; }

    
    [Range(0, 100, ErrorMessage = "The value must be between 0 and 100.")]
    public double ProgressValue { get; set; }
}
