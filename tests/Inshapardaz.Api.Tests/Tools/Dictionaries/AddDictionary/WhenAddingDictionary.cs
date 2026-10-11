using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.AddDictionary
{
    [TestFixture]
    public class WhenAddingDictionaryAsDictionaryWriter() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryView _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject("/dictionaries", new DictionaryView
            {
                Name = "Farhang-e-Asifia",
                Description = "Urdu to Urdu",
                SourceLanguage = "ur",
                TargetLanguage = "ur",
                EntryCount = 99,
                OwnerId = 12345
            });
            _view = await _response.GetContent<DictionaryView>();
            DictionaryBuilder.Track(_view.Id);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeCreated() => _response.ShouldBeCreated();

        [Test]
        public void ShouldBeOwnedByCurrentUserRegardlessOfPayload()
        {
            _view.OwnerId.Should().Be(AccountId);
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_view.Id).OwnerId.Should().Be(AccountId);
        }

        [Test]
        public void ShouldStartEmptyAsManualDictionary()
        {
            _view.EntryCount.Should().Be(0);
            _view.Format.Should().Be(DictionaryFormat.Manual);
        }

        [Test]
        public void ShouldSaveDetails()
        {
            var saved = Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_view.Id);
            saved.Name.Should().Be("Farhang-e-Asifia");
            saved.Description.Should().Be("Urdu to Urdu");
            saved.SourceLanguage.Should().Be("ur");
        }

        [Test]
        public void ShouldHaveEditingLinks()
        {
            _view.SelfLink().ShouldGet($"/dictionaries/{_view.Id}");
            _view.UpdateLink().ShouldPut($"/dictionaries/{_view.Id}");
            _view.DeleteLink().ShouldDelete($"/dictionaries/{_view.Id}");
            _view.Link(RelTypes.CreateArticle).ShouldPost($"/dictionaries/{_view.Id}/articles");
        }
    }

    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingDictionaryWithoutPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.PostObject("/dictionaries", new DictionaryView { Name = "Lughat" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();
    }

    [TestFixture]
    public class WhenAddingDictionaryAsAnonymous() : TestBase()
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.PostObject("/dictionaries", new DictionaryView { Name = "Lughat" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeUnauthorized() => _response.ShouldBeUnauthorized();
    }

    [TestFixture]
    public class WhenAddingDictionaryWithoutName() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup() => _response = await Client.PostObject("/dictionaries", new DictionaryView { Description = "no name" });

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }
}
