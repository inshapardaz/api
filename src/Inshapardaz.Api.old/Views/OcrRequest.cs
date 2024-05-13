using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views
{
    public class OcrRequest
    {
        [Required]
        public string Key { get; set; }
    }
}
