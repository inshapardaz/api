using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.Articles
{
    [TestFixture]
    public class WhenAddingArticleToOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryArticleView _view;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(AccountId).WithHeadwords("qalam").Build();
            _response = await Client.PostObject($"/dictionaries/{_dictionary.Id}/articles", new DictionaryArticleView
            {
                Headword = " kitab ",
                Pronunciation = "kitāb",
                Senses = [new WordSenseView { PartOfSpeech = "noun", Meanings = [new MeaningView { Definition = "book", Examples = ["ek kitab"] }] }],
                WordLinks = [new WordLinkView { LinkType = WordLinkType.Synonym, TargetWord = "pustak" }],
                Alternates = ["kitaab"]
            });
            _view = await _response.GetContent<DictionaryArticleView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeCreated() => _response.ShouldBeCreated();

        [Test]
        public void ShouldReturnArticle()
        {
            _view.Headword.Should().Be("kitab");
            _view.Senses.Single().Meanings.Single().Definition.Should().Be("book");
            _view.Senses.Single().Meanings.Single().Examples.Should().Equal("ek kitab");
            _view.WordLinks.Single().LinkType.Should().Be(WordLinkType.Synonym);
            _view.WordLinks.Single().TargetWord.Should().Be("pustak");
            _view.Alternates.Should().Equal("kitaab");
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _view.SelfLink().ShouldGet($"/dictionaries/{_dictionary.Id}/articles/{_view.Id}");
            _view.UpdateLink().Should().NotBeNull();
            _view.DeleteLink().Should().NotBeNull();
        }

        [Test]
        public void ShouldUpdateEntryCount() =>
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id).EntryCount.Should().Be(2);
    }

    [TestFixture]
    public class WhenAddingArticleToAnotherDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(null).Build();
            _response = await Client.PostObject($"/dictionaries/{_dictionary.Id}/articles", new DictionaryArticleView { Headword = "kitab" });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeForbidden() => _response.ShouldBeForbidden();

        [Test]
        public void ShouldNotHaveAddedArticle() =>
            Services.GetService<IDictionaryTestRepository>().GetArticles(_dictionary.Id).Should().BeEmpty();
    }

    [TestFixture]
    public class WhenSearchingArticles() : TestBase()
    {
        private PageView<DictionaryArticleView> _page;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.WithHeadwords("kitab", "kitabi", "qalam", "Kutub").Build();
            var response = await Client.GetAsync($"/dictionaries/{dictionary.Id}/articles?query=KIT&pageSize=1");
            response.ShouldBeOk();
            _page = await response.GetContent<PageView<DictionaryArticleView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldPageMatchingArticlesInHeadwordOrder()
        {
            _page.TotalCount.Should().Be(2);
            _page.Data.Single().Headword.Should().Be("kitab");
            _page.Link(RelTypes.Next).Should().NotBeNull();
        }

        [Test]
        public void ShouldNotOfferEditingToAnonymous()
        {
            _page.CreateLink().Should().BeNull();
            _page.Data.Single().UpdateLink().Should().BeNull();
        }
    }

    [TestFixture]
    public class WhenUpdatingArticleInOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryArticleDto _article;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.OwnedBy(AccountId).WithHeadwords("kitab").Build();
            _article = Services.GetService<IDictionaryTestRepository>().GetArticles(dictionary.Id).Single();
            _response = await Client.PutObject($"/dictionaries/{dictionary.Id}/articles/{_article.Id}", new DictionaryArticleView
            {
                Headword = "کتاب",
                RawDefinition = "book"
            });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSavedChanges()
        {
            var saved = Services.GetService<IDictionaryTestRepository>().GetArticles(_article.DictionaryId).Single();
            saved.Headword.Should().Be("کتاب");
            saved.HeadwordNormalised.Should().Be("کتاب");
            saved.RawDefinition.Should().Be("book");
        }
    }

    [TestFixture]
    public class WhenDeletingArticleFromOwnDictionary() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private HttpResponseMessage _response;
        private DictionaryDto _dictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(AccountId).WithHeadwords("kitab", "qalam").Build();
            var article = Services.GetService<IDictionaryTestRepository>().GetArticles(_dictionary.Id).First();
            _response = await Client.DeleteAsync($"/dictionaries/{_dictionary.Id}/articles/{article.Id}");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnNoContent() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedArticleAndUpdatedCount()
        {
            var repository = Services.GetService<IDictionaryTestRepository>();
            repository.GetArticles(_dictionary.Id).Select(a => a.Headword).Should().Equal("qalam");
            repository.GetDictionaryById(_dictionary.Id).EntryCount.Should().Be(1);
        }
    }
}
