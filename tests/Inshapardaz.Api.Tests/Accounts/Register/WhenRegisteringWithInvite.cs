using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.Register
{
    [TestFixture]
    public class WhenRegisteringWithInvite : TestBase
    {
        private LibraryDto _library;
        private AccountDto _account;
        private HttpResponseMessage _response;
        private string _name = RandomData.String;
        private string _password = RandomData.String;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = LibraryBuilder.Build();
            _account = AccountBuilder.InLibrary(_library.Id).AsInvitation().Build();

            _response = await Client.PostObject($"/accounts/register/{_account.InvitationCode}",
                new RegisterRequest
                {
                    Name = _name,
                    Password = _password,
                    AcceptTerms = true
                });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveActiveAccount()
        {
            AccountAssert.AssertAccountActive(_account.Id)
                .WithName(_name);
        }

        [Test]
        public async Task ShouldBeAbleToAuthenticate()
        {
            var response = await Client.PostObject("/accounts/authenticate", new AuthenticateRequest { Email = _account.Email, Password = _password });
            response.ShouldBeOk();
        }
    }
}