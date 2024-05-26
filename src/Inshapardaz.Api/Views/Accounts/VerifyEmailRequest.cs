using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Accounts;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}