using Inshapardaz.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Models.Accounts
{
    public class InviteUserRequest
    {
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Role Role { get; set; }
    }
}
