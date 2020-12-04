using Inshapardaz.Domain.Models;
using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class AccountDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool AcceptTerms { get; set; }

        public Role Role { get; set; }

        public DateTime Created { get; set; }
    }
}
