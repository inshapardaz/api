using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}