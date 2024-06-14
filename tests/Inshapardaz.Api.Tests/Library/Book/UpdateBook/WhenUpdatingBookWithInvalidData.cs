using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataBuilders;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookWithInvalidData
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

                var book = new BookView { Title = RandomData.Name, Authors = new List<AuthorView> { new AuthorView { Id = author.Id } } };

                _response = await Client.PutObject($"/libraries/{-RandomData.Number}/books/{book.Id}", book);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadRequestResult()
            {
                _response.ShouldBeBadRequest();
            }
        }

        [TestFixture]
        public class AndUpdatingWithNonExistingAuthor : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private AuthorDto _author;

            public AndUpdatingWithNonExistingAuthor() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, Authors = new List<AuthorView> { new AuthorView { Id = -RandomData.Number } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var authors = DatabaseConnection.GetAuthorsByBook(_bookToUpdate.Id);
                authors.Should().Contain(a => a.Id == _author.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithAuthorFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private AuthorDto _author;
            private LibraryDataBuilder _library2Builder;

            public AndUpdatingWithAuthorFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author2 = AuthorBuilder.WithLibrary(library2.Id).Build();

                _author = AuthorBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithAuthor(_author).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, Authors = new List<AuthorView> { new AuthorView { Id = author2.Id } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var authors = DatabaseConnection.GetAuthorsByBook(_bookToUpdate.Id);
                authors.Should().Contain(a => a.Id == _author.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithNonExistingSeries : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private SeriesDto _series;

            public AndUpdatingWithNonExistingSeries() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _series = SeriesBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, SeriesId = -RandomData.Number };

                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var book = DatabaseConnection.GetBookById(_bookToUpdate.Id);
                book.SeriesId.Should().Be(_series.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithSeriesFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private SeriesDto _series;
            private LibraryDataBuilder _library2Builder;

            public AndUpdatingWithSeriesFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series2 = SeriesBuilder.WithLibrary(library2.Id).Build();

                _series = SeriesBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithSeries(_series).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, SeriesId = series2.Id };
                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var book = DatabaseConnection.GetBookById(_bookToUpdate.Id);
                book.SeriesId.Should().Be(_series.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithNonExistingCategory : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private CategoryDto _category;

            public AndUpdatingWithNonExistingCategory() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _category = CategoryBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, Categories = new[] { new CategoryView { Id = -RandomData.Number } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var categories = DatabaseConnection.GetCategoriesByBook(_bookToUpdate.Id);
                categories.Should().HaveCount(1);
                categories.First().Id.Should().Be(_category.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithCategoryFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private BookDto _bookToUpdate;
            private CategoryDto _category;
            private LibraryDataBuilder _library2Builder;

            public AndUpdatingWithCategoryFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series2 = CategoryBuilder.WithLibrary(library2.Id).Build();

                _category = CategoryBuilder.WithLibrary(LibraryId).Build();

                var books = BookBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = RandomData.Text, Categories = new[] { new CategoryView { Id = _category.Id } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_bookToUpdate.Id}", book);
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

            [Test]
            public void ShouldNotUpdateTheAuthor()
            {
                var categories = DatabaseConnection.GetCategoriesByBook(_bookToUpdate.Id);
                categories.Should().HaveCount(1);
                categories.First().Id.Should().Be(_category.Id);
            }
        }
    }
}
