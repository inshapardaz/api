using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenRegisteringASuperUserWhoIsAlreadyRegistered : TestBase
    {
        private HttpResponseMessage _response;

        public WhenRegisteringASuperUserWhoIsAlreadyRegistered() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.Build();
            var account = AccountBuilder.As(Role.Admin)
                .AsInvitation()
                .Unverified()
                .Build();

            _response = await Client.PostObject($"/accounts/invite",
                new InviteUserRequest
                {
                    Name = RandomData.String,
                    Email = account.Email,
                    Role = Role.Admin
                });
        }

        [Test]
        public void ShouldReturnConflict()
        {
            _response.ShouldBeConflict();
        }
    }
}
