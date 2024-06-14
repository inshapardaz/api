using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Accounts.CheckInvitationCode
{
    [TestFixture]
    public class WhenCheckingValidInvitation : TestBase
    {
        private AccountDto _account;
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _account = AccountBuilder.Verified().AsInvitation().Build();
            _response = await Client.GetAsync($"/accounts/invitation/{_account.InvitationCode}");
        }

        [Test]
        public void ShouldReturnOK()
        {
            _response.ShouldBeOk();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Cleanup();
        }
    }
}