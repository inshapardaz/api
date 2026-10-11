using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.GetDictionary
{
    [TestFixture]
    public class WhenGettingDictionaryAsAnonymous() : TestBase()
    {
        private HttpResponseMessage _response;
        private DictionaryView _view;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.WithHeadwords("kitab", "qalam").Build();
            _response = await Client.GetAsync($"/dictionaries/{_dictionary.Id}");
            _view = await _response.GetContent<DictionaryView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnDictionary()
        {
            _view.Name.Should().Be(_dictionary.Name);
            _view.EntryCount.Should().Be(2);
        }

        [Test]
        public void ShouldHaveReadLinks()
        {
            _view.SelfLink().ShouldGet($"/dictionaries/{_dictionary.Id}");
            _view.Link(RelTypes.Articles).ShouldGet($"/dictionaries/{_dictionary.Id}/articles");
            _view.Link(RelTypes.Download).ShouldGet($"/dictionaries/{_dictionary.Id}/export");
        }

        [Test]
        public void ShouldNotHaveEditingLinks()
        {
            _view.UpdateLink().Should().BeNull();
            _view.DeleteLink().Should().BeNull();
            _view.Link(RelTypes.CreateArticle).Should().BeNull();
        }
    }

    [TestFixture]
    public class WhenGettingOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private DictionaryView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.OwnedBy(AccountId).Build();
            _view = await (await Client.GetAsync($"/dictionaries/{dictionary.Id}")).GetContent<DictionaryView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveEditingLinks()
        {
            _view.UpdateLink().Should().NotBeNull();
            _view.DeleteLink().Should().NotBeNull();
            _view.Link(RelTypes.CreateArticle).Should().NotBeNull();
            _view.Link(RelTypes.Merge).Should().NotBeNull();
        }
    }

    [TestFixture]
    public class WhenGettingDictionaryOwnedByAnotherWriter() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private DictionaryView _view;
        private AccountDto _owner;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _owner = Services.GetService<AccountDataBuilder>().As(Role.Reader).AsDictionaryWriter().Verified().Build();
            var dictionary = DictionaryBuilder.OwnedBy(_owner.Id).Build();
            _view = await (await Client.GetAsync($"/dictionaries/{dictionary.Id}")).GetContent<DictionaryView>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
            Services.GetService<IAccountTestRepository>().DeleteAccount(_owner.Id);
        }

        [Test]
        public void ShouldNotHaveEditingLinks()
        {
            _view.UpdateLink().Should().BeNull();
            _view.DeleteLink().Should().BeNull();
        }
    }

    [TestFixture]
    public class WhenGettingDictionaryAsAdmin() : TestBase(Role.Admin)
    {
        private DictionaryView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.OwnedBy(null).Build();
            _view = await (await Client.GetAsync($"/dictionaries/{dictionary.Id}")).GetContent<DictionaryView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveEditingLinks()
        {
            _view.UpdateLink().Should().NotBeNull();
            _view.DeleteLink().Should().NotBeNull();
        }
    }

    [TestFixture]
    public class WhenGettingDictionaryThatDoesNotExist() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.GetAsync($"/dictionaries/{-RandomData.Number}");

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNotFound() => _response.ShouldBeNotFound();
    }

    [TestFixture]
    public class WhenGettingDictionaries() : TestBase()
    {
        private PageView<DictionaryView> _page;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.WithName($"Lughat {Guid.NewGuid():N}").Build();
            var response = await Client.GetAsync($"/dictionaries?query={_dictionary.Name}");
            response.ShouldBeOk();
            _page = await response.GetContent<PageView<DictionaryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldFindDictionaryByName() => _page.Data.Should().ContainSingle(d => d.Id == _dictionary.Id);

        [Test]
        public void ShouldNotOfferCreateLinkToAnonymous() => _page.CreateLink().Should().BeNull();
    }

    [TestFixture]
    public class WhenGettingDictionariesAsDictionaryWriter() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private PageView<DictionaryView> _page;

        [OneTimeSetUp]
        public async Task Setup() => _page = await (await Client.GetAsync("/dictionaries")).GetContent<PageView<DictionaryView>>();

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldOfferCreateAndUploadLinks()
        {
            _page.CreateLink().ShouldPost("/dictionaries");
            _page.Link(RelTypes.BulkUpload).ShouldPost("/dictionaries/upload");
        }
    }
}
