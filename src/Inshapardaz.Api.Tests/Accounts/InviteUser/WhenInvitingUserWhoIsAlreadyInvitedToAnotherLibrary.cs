using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenInvitingUserWhoIsAlreadyInvitedToAnotherLibrary : TestBase
    {
        private HttpResponseMessage _response;
        private Dto.LibraryDto _library1, _library2;
        private Dto.AccountDto _account;

        public WhenInvitingUserWhoIsAlreadyInvitedToAnotherLibrary() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library1 = LibraryBuilder.Build();
            _account = AccountBuilder.As(Role.Reader)
                .InLibrary(_library1.Id)
                .AsInvitation()
                .Build();

            _library2 = LibraryBuilder.Build();

            _response = await Client.PostObject($"/accounts/invite/library/{_library2.Id}",
                new InviteUserRequest
                {
                    Email = _account.Email,
                    Role = Role.LibraryAdmin
                });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void UserShouldBeAddedToBothLibraries()
        {
            AccountAssert.UserInLibrary(_account.Id, _library1.Id, Role.Reader);
            AccountAssert.UserInLibrary(_account.Id, _library2.Id, Role.LibraryAdmin);
        }
    }
}