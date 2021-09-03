using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenRegisteringAsSuperUser : TestBase
    {
        private HttpResponseMessage _response;
        private string _name = RandomData.String;
        private string _email = RandomData.Email;
        private LibraryDto _library;

        public WhenRegisteringAsSuperUser() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = LibraryBuilder.Build();

            _response = await Client.PostObject($"/api/accounts/invite/library/{_library.Id}",
                new InviteUserRequest
                {
                    Name = _name,
                    Email = _email,
                    Role = Role.Reader
                });
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCreatedAccount()
        {
            AccountAssert.AssertAccountExistsWithEmail(_email)
                .WithName(_name)
                .ShouldBeInvited()
                .ShouldHaveInvitationExpiring(DateTime.Today.AddDays(7))
                .ShouldNotBeVerified()
                .InLibrary(_library.Id);
        }

        [Test]
        public void ShouldHaveSentEmailToUser()
        {
            SmtpClient.AssertEmailSentTo(_email)
                .WithSubject($"Welcome to {_library.Name}")
                .WithBodyContainting($"You are invited to join {_library.Name}");
        }
    }
}
