using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticle
{
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingArticleWithMultipleAuthors : TestBase
    {
        private ArticleAssert _articleAssert;
        private HttpResponseMessage _response;

        public WhenAddingArticleWithMultipleAuthors(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var reviewer = AccountBuilder.As(Role.Writer).Build();
            var writer = AccountBuilder.As(Role.Writer).Build();
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(3);
            var categories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var article = new ArticleView
            {
                Title = RandomData.Name,
                Authors = authors.PickRandom(2).Select(c => new AuthorView { Id = c.Id }),
                Categories = categories.PickRandom(2).Select(c => new CategoryView { Id = c.Id }),
                IsPublic = true,
                WriterAccountId = writer.Id,
                WriterAccountName = writer.Name,
                WriterAssignTimeStamp = DateTime.UtcNow,
                ReviewerAccountId = reviewer.Id,
                ReviewerAccountName = reviewer.Name,
                ReviewerAssignTimeStamp = DateTime.UtcNow,
                Status = EditingStatus.Available.ToString()
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            _articleAssert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
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
        public void ShouldSaveArticle()
        {
            _articleAssert.ShouldHaveSavedArticle();
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
