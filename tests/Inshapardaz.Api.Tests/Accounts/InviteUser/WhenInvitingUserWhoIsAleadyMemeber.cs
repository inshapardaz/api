using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenInvitingUserWhoIsAleadyMemeber() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.Build();
            var account = AccountBuilder.As(Role.Reader)
                .InLibrary(library.Id)
                .Verified()
                .Build();

            _response = await Client.PostObject($"/accounts/invite/library/{library.Id}",
                new InviteUserRequest
                {
                    Email = account.Email,
                    Role = Role.Reader
                });
        }

        [Test]
        public void ShouldReturnCoflict() => _response.ShouldBeConflict();
    }
}
