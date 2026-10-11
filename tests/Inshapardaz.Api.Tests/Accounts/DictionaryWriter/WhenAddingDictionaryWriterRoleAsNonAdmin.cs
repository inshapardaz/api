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
    [TestFixture(Role.Reader, false)]
    [TestFixture(Role.Writer, false)]
    [TestFixture(Role.LibraryAdmin, false)]
    [TestFixture(Role.Reader, true)]
    public class WhenAddingDictionaryWriterRoleAsNonAdmin(Role role, bool isDictionaryWriter) : TestBase(role, isDictionaryWriter: isDictionaryWriter)
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
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotBeDictionaryWriter() =>
            Services.GetService<IAccountTestRepository>().GetAccountById(_otherAccount.Id).IsDictionaryWriter.Should().BeFalse();
    }
}
