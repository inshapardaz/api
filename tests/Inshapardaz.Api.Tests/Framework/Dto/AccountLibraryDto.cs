using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class AccountLibraryDto
    {
        public int AccountId { get; set; }
        public int LibraryId { get; set; }

        public Role Role { get; set; }
    }
}
