using Inshapardaz.Api.Views.Accounts;
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
    public class WhenRegisteringNewSuperUser : TestBase
    {
        private HttpResponseMessage _response;
        private string _name = RandomData.String;
        private string _email = RandomData.Email;
        private LibraryDto _library;

        public WhenRegisteringNewSuperUser() : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _library = LibraryBuilder.Build();

            _response = await Client.PostObject($"/accounts/invite",
                new InviteUserRequest
                {
                    Name = _name,
                    Email = _email,
                    Role = Role.Admin
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
                .ShouldBeSuperAdmin()
                .InNoLibrary();
        }

        [Test]
        public void ShouldHaveSentEmailToUser()
        {
            SmtpClient.AssertEmailSentTo(_email)
                .WithSubject($"Welcome to Dashboards")
                .WithBodyContainting("You are invited to join team of our administrators");
        }
    }
}