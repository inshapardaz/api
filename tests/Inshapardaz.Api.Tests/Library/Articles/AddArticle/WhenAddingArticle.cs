using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticle
{
    [TestFixture]
    public class WhenAddingArticle : TestBase
    {
        private ArticleAssert _articleAssert;
        private HttpResponseMessage _response;

        public WhenAddingArticle() : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            var article = new ArticleView
            {
                Title = RandomData.Name,
                Authors = new List<AuthorView> { new AuthorView { Id = author.Id } }
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            _articleAssert = ArticleAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _articleAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheArticle()
        {
            _articleAssert.ShouldHaveSavedArticle(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _articleAssert.ShouldHaveSelfLink()
                        .ShouldHaveUpdateLink()
                        .ShouldHaveDeleteLink();
        }
    }
}
