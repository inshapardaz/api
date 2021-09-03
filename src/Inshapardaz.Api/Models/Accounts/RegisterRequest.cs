using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Models.Accounts
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
    }
}
