using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.UpdateDictionary
{
    [TestFixture]
    public class WhenUpdatingOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(AccountId).Build();
            _response = await Client.PutObject($"/dictionaries/{_dictionary.Id}", new DictionaryView { Name = "Renamed", TargetLanguage = "hi" });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSavedChanges()
        {
            var saved = Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id);
            saved.Name.Should().Be("Renamed");
            saved.TargetLanguage.Should().Be("hi");
            saved.OwnerId.Should().Be(AccountId);
        }
    }

    [TestFixture]
    public class WhenUpdatingDictionaryAsAdmin() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(null).Build();
            _response = await Client.PutObject($"/dictionaries/{_dictionary.Id}", new DictionaryView { Name = "Renamed by admin" });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSavedChanges() =>
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id).Name.Should().Be("Renamed by admin");
    }

    [TestFixture]
    public class WhenUpdatingDictionaryOwnedByAnotherWriter() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;
        private AccountDto _owner;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _owner = Services.GetService<AccountDataBuilder>().As(Role.Reader).AsDictionaryWriter().Verified().Build();
            _dictionary = DictionaryBuilder.OwnedBy(_owner.Id).Build();
            _response = await Client.PutObject($"/dictionaries/{_dictionary.Id}", new DictionaryView { Name = "Hijacked" });
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
            Services.GetService<IAccountTestRepository>().DeleteAccount(_owner.Id);
        }

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveChangedDictionary() =>
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id).Name.Should().Be(_dictionary.Name);
    }

    // Owner who has since lost the DictionaryWriter role.
    [TestFixture]
    public class WhenUpdatingOwnDictionaryAfterRoleRevoked() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.OwnedBy(AccountId).Build();
            _response = await Client.PutObject($"/dictionaries/{dictionary.Id}", new DictionaryView { Name = "Renamed" });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }

    [TestFixture]
    public class WhenDeletingOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(AccountId).WithHeadwords("kitab", "qalam").Build();
            _response = await Client.DeleteAsync($"/dictionaries/{_dictionary.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedDictionaryAndArticles()
        {
            var repository = Services.GetService<IDictionaryTestRepository>();
            repository.GetDictionaryById(_dictionary.Id).Should().BeNull();
            repository.GetArticles(_dictionary.Id).Should().BeEmpty();
        }
    }

    [TestFixture]
    public class WhenDeletingDictionaryWithoutOwnership() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(null).Build();
            _response = await Client.DeleteAsync($"/dictionaries/{_dictionary.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveDeletedDictionary() =>
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id).Should().NotBeNull();
    }
}
