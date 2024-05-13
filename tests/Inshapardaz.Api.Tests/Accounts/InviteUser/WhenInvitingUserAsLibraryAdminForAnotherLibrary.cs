using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenInvitingUserAsLibraryAdminForAnotherLibrary : TestBase
    {
        private HttpResponseMessage _response;

        public WhenInvitingUserAsLibraryAdminForAnotherLibrary() : base(Role.LibraryAdmin)
        {
        }

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
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }
    }
}
