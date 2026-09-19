using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.Delete
{
    [TestFixture]
    public class WhenNonAdminDeletesAnotherAccount() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private AccountDto _otherAccount;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _otherAccount = Services.GetService<AccountDataBuilder>().As(Role.Reader).Verified().Build();

            _response = await Client.DeleteAsync($"/accounts/{_otherAccount.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Services.GetService<IAccountTestRepository>().DeleteAccount(_otherAccount.Id);

        [Test]
        public void ShouldReturnUnauthorized() => _response.ShouldBeUnauthorized();

        [Test]
        public void ShouldNotAnonymizeOtherAccount()
        {
            var dbAccount = Services.GetService<IAccountTestRepository>().GetAccountById(_otherAccount.Id);
            dbAccount.IsDeleted.Should().BeFalse();
        }
    }
}
