using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Accounts
{
    public class ChangePasswordRequest
    {
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string OldPassword { get; set; }
    }
}
