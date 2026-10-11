using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Accounts.DictionaryWriter
{
    [TestFixture]
    public class WhenAddingDictionaryWriterRoleAsAdmin() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private AccountDto _otherAccount;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _otherAccount = Services.GetService<AccountDataBuilder>().As(Role.Reader).Verified().Build();

            _response = await Client.PutAsync($"/accounts/{_otherAccount.Id}/roles/dictionary-writer", null);
        }

        [OneTimeTearDown]
        public void Teardown() => Services.GetService<IAccountTestRepository>().DeleteAccount(_otherAccount.Id);

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldBeDictionaryWriter() =>
            Services.GetService<IAccountTestRepository>().GetAccountById(_otherAccount.Id).IsDictionaryWriter.Should().BeTrue();
    }
}
