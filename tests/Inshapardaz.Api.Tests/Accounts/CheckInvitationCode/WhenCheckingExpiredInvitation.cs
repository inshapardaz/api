using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.CheckInvitationCode
{
    [TestFixture]
    public class WhenCheckingExpiredInvitation : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Verified()
                .AsInvitation().ExpiringInvitation(DateTime.Today.AddDays(-1))
                .Build();
            _response = await Client.GetAsync($"/accounts/invitation/{account.InvitationCode}");
        }

        [Test]
        public void ShouldReturnGone()
        {
            _response.ShouldBeGone();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Cleanup();
        }
    }
}