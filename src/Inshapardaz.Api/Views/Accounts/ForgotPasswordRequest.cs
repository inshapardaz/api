using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
