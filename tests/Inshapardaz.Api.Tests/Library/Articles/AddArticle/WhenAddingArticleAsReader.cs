using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticle
{
    [TestFixture]
    public class WhenAddingArticleAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();
            var article = new ArticleView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

            _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
