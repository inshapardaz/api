using Inshapardaz.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inshapardaz.Api.Views.Accounts
{
    public class InviteUserRequest
    {
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
    }
}
