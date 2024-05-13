using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}