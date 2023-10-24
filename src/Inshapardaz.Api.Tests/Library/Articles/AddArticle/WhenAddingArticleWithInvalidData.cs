using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticle
{
    [TestFixture]
    public partial class WhenAddingArticleWithInvalidData
    {
        [TestFixture]
        public class AndUsingNonExistingLibrary : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var article = new ArticleView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

                _response = await Client.PostObject($"/libraries/{-RandomData.Number}/articles", article);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldReturnBadRequest()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingAuthorFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingAuthorFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author = AuthorBuilder.WithLibrary(library2.Id).Build();

                var article = new ArticleView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

                _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingNonExistingAuthor : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingAuthor() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var article = new ArticleView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = -RandomData.Number } } };

                _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }
        
        [TestFixture]
        public class AndUsingCategoryFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingCategoryFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author = AuthorBuilder.WithLibrary(Library.Id).Build();
                var categories = CategoryBuilder.WithLibrary(library2.Id).Build();

                var article = new ArticleView { 
                    Title = RandomData.Name, 
                    Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                    Categories = new List<CategoryView> { new CategoryView { Id = categories.Id } }
                };

                _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUsingNonExistingCategories : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingCategories() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(Library.Id).Build();

                var article = new ArticleView { 
                    Title = RandomData.Name, 
                    Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                    Categories = new List<CategoryView> { new CategoryView { Id = -RandomData.Number } } 
                };

                _response = await Client.PostObject($"/libraries/{LibraryId}/articles", article);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }
    }
}
