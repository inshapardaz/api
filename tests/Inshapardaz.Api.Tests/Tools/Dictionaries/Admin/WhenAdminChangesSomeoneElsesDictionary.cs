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

namespace Inshapardaz.Api.Tests.Tools.Dictionaries.Admin
{
    // Admins manage every dictionary, including ones owned by a dictionary writer.
    [TestFixture]
    public class WhenAdminChangesSomeoneElsesDictionary() : TestBase(Role.Admin)
    {
        private AccountDto _owner;
        private DictionaryDto _dictionary;
        private DictionaryDto _deleted;
        private HttpResponseMessage _addArticle;
        private HttpResponseMessage _updateArticle;
        private HttpResponseMessage _deleteArticle;
        private HttpResponseMessage _merge;
        private HttpResponseMessage _deleteDictionary;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _owner = Services.GetService<AccountDataBuilder>().As(Role.Reader).AsDictionaryWriter().Verified().Build();
            _dictionary = DictionaryBuilder.OwnedBy(_owner.Id).WithHeadwords("bank 1", "bank 2", "kitab", "qalam").Build();
            _deleted = DictionaryBuilder.OwnedBy(_owner.Id).WithHeadwords("river").Build();
            var articles = Services.GetService<IDictionaryTestRepository>().GetArticles(_dictionary.Id).ToList();

            _addArticle = await Client.PostObject($"/dictionaries/{_dictionary.Id}/articles", new DictionaryArticleView { Headword = "dawat" });
            _updateArticle = await Client.PutObject($"/dictionaries/{_dictionary.Id}/articles/{articles.Single(a => a.Headword == "kitab").Id}",
                new DictionaryArticleView { Headword = "kitab", RawDefinition = "book" });
            _deleteArticle = await Client.DeleteAsync($"/dictionaries/{_dictionary.Id}/articles/{articles.Single(a => a.Headword == "qalam").Id}");
            _merge = await Client.PostObject($"/dictionaries/{_dictionary.Id}/merge", new MergeArticlesView
            {
                ArticleIds = articles.Where(a => a.Headword.StartsWith("bank")).Select(a => a.Id).ToList()
            });
            _deleteDictionary = await Client.DeleteAsync($"/dictionaries/{_deleted.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
            Services.GetService<IAccountTestRepository>().DeleteAccount(_owner.Id);
        }

        [Test]
        public void ShouldAddArticle() => _addArticle.ShouldBeCreated();

        [Test]
        public void ShouldUpdateArticle() => _updateArticle.ShouldBeOk();

        [Test]
        public void ShouldDeleteArticle() => _deleteArticle.ShouldBeNoContent();

        [Test]
        public void ShouldMergeArticles() => _merge.ShouldBeOk();

        [Test]
        public void ShouldHaveSavedArticleChanges() =>
            Services.GetService<IDictionaryTestRepository>().GetArticles(_dictionary.Id).Select(a => a.Headword)
                .Should().BeEquivalentTo("bank", "kitab", "dawat");

        [Test]
        public void ShouldKeepOwnership() =>
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_dictionary.Id).OwnerId.Should().Be(_owner.Id);

        [Test]
        public void ShouldDeleteDictionary()
        {
            _deleteDictionary.ShouldBeNoContent();
            Services.GetService<IDictionaryTestRepository>().GetDictionaryById(_deleted.Id).Should().BeNull();
        }
    }
}
