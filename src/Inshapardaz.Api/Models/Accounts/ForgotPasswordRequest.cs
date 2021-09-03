using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
