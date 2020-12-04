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

namespace Inshapardaz.Api.Tests.Library.Book.AddBook
{
    [TestFixture]
    public class WhenAddingBookWithInvalidData
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
                var book = new BookView { Title = Random.Name, AuthorId = author.Id };

                _response = await Client.PostObject($"/library/{-Random.Number}/books", book);
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
        public class AndUsingNonExistingAuthor : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingAuthor() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var book = new BookView { Title = Random.Name, AuthorId = -Random.Number };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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

                var book = new BookView { Title = Random.Name, AuthorId = author.Id };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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
        public class AndUsingNonExistingSeries : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingSeries() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id, SeriesId = -Random.Number };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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
        public class AndUsingSeriesFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingSeriesFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series = SeriesBuilder.WithLibrary(library2.Id).Build();
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id, SeriesId = series.Id };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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
        public class AndUsingNonExistingCategory : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingCategory() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();
                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    AuthorId = author.Id,
                    Categories = new CategoryView[] { new CategoryView { Id = -Random.Number } }
                };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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
        public class AndUsingCategorysFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private LibraryDataBuilder _library2Builder;

            public AndUsingCategorysFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var category = CategoryBuilder.WithLibrary(library2.Id).Build();
                var author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    AuthorId = author.Id,
                    Categories = new CategoryView[] { new CategoryView { Id = category.Id } }
                };

                _response = await Client.PostObject($"/library/{LibraryId}/books", book);
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
    }
}
