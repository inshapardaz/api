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
    public class WhenInvitingUserToLibraryDoesNotExists : TestBase
    {
        private HttpResponseMessage _response;

        public WhenInvitingUserToLibraryDoesNotExists() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = LibraryBuilder.Build();
            var account = AccountBuilder.As(Role.Reader)
                .InLibrary(library.Id)
                .Verified()
                .Build();

            _response = await Client.PostObject($"/accounts/invite/library/{-RandomData.Number}",
                new InviteUserRequest
                {
                    Email = account.Email,
                    Role = Role.Reader
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}