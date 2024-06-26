using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Accounts;

public class AuthenticateRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
