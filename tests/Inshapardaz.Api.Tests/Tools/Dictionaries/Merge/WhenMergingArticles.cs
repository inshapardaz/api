using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Tools;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.Merge
{
    [TestFixture]
    public class WhenMergingArticles() : TestBase(Role.Reader, isDictionaryWriter: true)
    {
        private DictionaryDto _dictionary;
        private List<MergeCandidateGroupView> _candidates;
        private HttpResponseMessage _mergeResponse;
        private DictionaryArticleView _merged;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dictionary = DictionaryBuilder.OwnedBy(AccountId).WithHeadwords("bank 1", "bank (2)", "river").Build();

            var candidatesResponse = await Client.GetAsync($"/dictionaries/{_dictionary.Id}/merge-candidates");
            candidatesResponse.ShouldBeOk();
            _candidates = (await candidatesResponse.GetContent<IEnumerable<MergeCandidateGroupView>>()).ToList();

            _mergeResponse = await Client.PostObject($"/dictionaries/{_dictionary.Id}/merge", new MergeArticlesView
            {
                ArticleIds = _candidates.Single().Articles.Select(a => a.Id).Order().ToList()
            });
            _merged = await _mergeResponse.GetContent<DictionaryArticleView>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldFindNumberedHomographs()
        {
            _candidates.Single().BaseHeadword.Should().Be("bank");
            _candidates.Single().Articles.Select(a => a.Headword).Should().BeEquivalentTo("bank 1", "bank (2)");
        }

        [Test]
        public void ShouldReturnOk() => _mergeResponse.ShouldBeOk();

        [Test]
        public void ShouldCombineIntoOneArticle()
        {
            _merged.Headword.Should().Be("bank");
            _merged.Senses.Should().HaveCount(2);
            _merged.RawDefinition.Should().Be("meaning of bank 1<hr>meaning of bank (2)");
        }

        [Test]
        public void ShouldRemoveMergedArticles()
        {
            var repository = Services.GetService<IDictionaryTestRepository>();
            repository.GetArticles(_dictionary.Id).Select(a => a.Headword).Should().BeEquivalentTo("bank", "river");
            repository.GetDictionaryById(_dictionary.Id).EntryCount.Should().Be(2);
        }
    }

    [TestFixture]
    public class WhenMergingFewerThanTwoArticles() : TestBase(Role.Admin)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dictionary = DictionaryBuilder.WithHeadwords("bank 1").Build();
            var article = Services.GetService<IDictionaryTestRepository>().GetArticles(dictionary.Id).Single();
            _response = await Client.PostObject($"/dictionaries/{dictionary.Id}/merge", new MergeArticlesView { ArticleIds = [article.Id] });
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldBeBadRequest() => _response.ShouldBeBadRequest();
    }
}
