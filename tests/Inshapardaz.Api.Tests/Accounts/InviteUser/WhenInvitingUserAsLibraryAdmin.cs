using Inshapardaz.Api.Views.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.InviteUser
{
    [TestFixture]
    public class WhenInvitingUserAsLibraryAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private string _name = RandomData.String;
        private string _email = RandomData.Email;

        public WhenInvitingUserAsLibraryAdmin() : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject($"/accounts/invite/library/{Library.Id}",
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
                .ShouldBeInRole(Role.Reader, Library.Id)
                .InLibrary(Library.Id);
        }

        [Test]
        public void ShouldHaveSentEmailToAdministrator()
        {
            SmtpClient.AssertEmailSentTo(_email)
                .WithSubject($"Welcome to {Library.Name}")
                .WithBodyContainting($"You are invited to join {Library.Name}");
        }
    }
}