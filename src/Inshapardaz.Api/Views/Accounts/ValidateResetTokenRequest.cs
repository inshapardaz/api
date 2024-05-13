using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}