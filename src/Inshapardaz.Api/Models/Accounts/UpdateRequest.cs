using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Models.Accounts
{
    public class UpdateRequest
    {
        private string _password;
        private string _confirmPassword;
        private string _email;

        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role {get; set;}

        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
