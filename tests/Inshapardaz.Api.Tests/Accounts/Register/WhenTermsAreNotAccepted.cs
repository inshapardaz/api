using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Register
{
    [TestFixture]
    public class WhenTermsAreNotAccepted : TestBase
    {
        private LibraryDto _library;
        private AccountDto _account;
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var name = RandomData.String;
            var password = RandomData.String;
            _library = LibraryBuilder.Build();
            _account = AccountBuilder.InLibrary(_library.Id).AsInvitation().Build();

            _response = await Client.PostObject($"/accounts/register/{_account.InvitationCode}",
                new RegisterRequest
                {
                    Name = name,
                    Password = password,
                    AcceptTerms = false
                });
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            _response.ShouldBeBadRequest();
        }
    }
}