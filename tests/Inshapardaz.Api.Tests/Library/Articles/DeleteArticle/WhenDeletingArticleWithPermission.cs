using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingArticleWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private ArticleDto _expected;
        private IEnumerable<ArticleContentDto> _contents;
        private int _authorId;

        public WhenDeletingArticleWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .WithContents(2)
                                    .Build(1)
                                    .Single();

            _contents = ArticleTestRepository.GetArticleContents(_expected.Id);
            _authorId = ArticleBuilder.Authors.First().Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{_expected.Id}");
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedArticle()
        {
            _assert.ShouldHaveDeletedArticle(_expected.Id);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            AuthorTestRepository.GetAuthorById(_authorId).Should().NotBeNull();
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = CategoryTestRepository.GetCategoriesByArticle(_expected.Id);
            var catAssert = Services.GetService<CategoryAssert>().ForLibrary(LibraryId);
            cats.ForEach(cat => catAssert.ShouldNotHaveDeletedCategory(cat.Id));
        }

        [Test]
        public void ShouldBeDeletedFromTheFavoritesOfAllUsers()
        {
            _assert.ShouldNotBeInFavorites(_expected.Id, AccountId);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadArticles()
        {
            _assert.ShouldHaveDeletedArticleFromRecentReads(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheArticleImage()
        {
            var imageFile = ArticleBuilder.Files.Single(x => x.Id == _expected.ImageId);
            FileAssert.FileDoesnotExist(imageFile);
        }

        [Test]
        public void ShouldHaveDeletedArticleContents()
        {
            _assert.ShouldHaveDeletedArticleContents(_expected.Id);
            ArticleBuilder.Files.Where(x => _contents.Any(y => y.FileId == x.Id))
                .ForEach(x => FileAssert.FileDoesnotExist(x));
        }
    }
}
