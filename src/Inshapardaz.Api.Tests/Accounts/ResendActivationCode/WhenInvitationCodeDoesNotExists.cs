﻿using Inshapardaz.Api.Models.Accounts;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.ResendActivationCode
{
    [TestFixture]
    public class WhenInvitationCodeDoesNotExists : TestBase
    {
        private HttpResponseMessage _response;

        public WhenInvitationCodeDoesNotExists()
            : base(Domain.Models.Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject($"/accounts/invitations", new ResendInvitationCodeRequest() { Email = RandomData.Email });
        }

        [Test]
        public void ShouldReturnOK()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSentEmailToUser()
        {
            SmtpClient.AssertNoEmailSent();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Cleanup();
        }
    }
}