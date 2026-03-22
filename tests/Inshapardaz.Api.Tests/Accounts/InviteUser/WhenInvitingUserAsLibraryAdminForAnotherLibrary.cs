using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenInvitingUserAsLibraryAdminForAnotherLibrary() : TestBase(Role.LibraryAdmin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.WithOutAccount().Build();

            _response = await Client.PostObject($"/accounts/invite/library/{library.Id}",
                new InviteUserRequest
                {
                    Email = RandomData.Email,
                    Role = Role.Reader
                });
        }

        [Test]
        public void ShouldReturnForbidden() => _response.ShouldBeForbidden();
    }
}
