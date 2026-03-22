using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.ResendActivationCode
{
    [TestFixture]
    public class WhenResendingInvitationCode() : TestBase(Domain.Models.Role.Reader)
    {
        private LibraryDto _library;
        private AccountDto _account;
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = LibraryBuilder.Build();
            _account = AccountBuilder.InLibrary(_library.Id).AsInvitation().Build();
            _response = await Client.PostObject($"/accounts/invitations", new ResendInvitationCodeRequest() { Email = _account.Email });
        }

        [Test]
        public void ShouldReturnOK() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSentEmailToUser()
        {
            SmtpClient.AssertEmailSentTo(_account.Email)
                .WithSubject($"Welcome to {_library.Name}")
                .WithBodyContainting($"You are invited to join {_library.Name}");
        }

        [OneTimeTearDown]
        public void TearDown() => Cleanup();
    }
}
