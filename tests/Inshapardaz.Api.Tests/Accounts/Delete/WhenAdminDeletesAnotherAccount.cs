using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.Delete
{
    [TestFixture]
    public class WhenAdminDeletesAnotherAccount() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private AccountDto _otherAccount;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _otherAccount = Services.GetService<AccountDataBuilder>().As(Role.Reader).Verified().Build();

            _response = await Client.DeleteAsync($"/accounts/{_otherAccount.Id}");
        }

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldAnonymizeAccount() => AccountAssert.ShouldBeAnonymized(_otherAccount.Id);
    }
}
