using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookWithInvalidData
    {
        [TestFixture]
        public class AndUsingNonExistingLibrary : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id };

                _response = (BadRequestResult)await handler.Run(book, -Random.Number, 0, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUpdatingWithNonExistingAuthor : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private AuthorDto _author;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithAuthor(_author).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, AuthorId = -Random.Number };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
                book.AuthorId.Should().Be(_author.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithAuthorFromOtherLibrary : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private AuthorDto _author;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author2 = Container.GetService<AuthorsDataBuilder>().WithLibrary(library2.Id).Build();

                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithAuthor(_author).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, AuthorId = author2.Id };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
                book.AuthorId.Should().Be(_author.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithNonExistingSeries : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private SeriesDto _series;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _series = Container.GetService<SeriesDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithSeries(_series).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, SeriesId = -Random.Number };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUpdatingWithSeriesFromOtherLibrary : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private SeriesDto _series;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series2 = Container.GetService<SeriesDataBuilder>().WithLibrary(library2.Id).Build();

                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _series = Container.GetService<SeriesDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithSeries(_series).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, SeriesId = series2.Id };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUpdatingWithNonExistingCategory : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private CategoryDto _category;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _category = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, Categories = new[] { new CategoryView { Id = -Random.Number } } };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUpdatingWithCategoryFromOtherLibrary : LibraryTest<Functions.Library.Books.UpdateBook>
        {
            private BadRequestResult _response;
            private BookDto _bookToUpdate;
            private CategoryDto _category;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series2 = Container.GetService<CategoriesDataBuilder>().WithLibrary(library2.Id).Build();

                var dataBuilder = Container.GetService<BooksDataBuilder>();
                _category = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build();

                var books = dataBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _bookToUpdate = books.PickRandom();

                var book = new BookView { Title = Random.Text, Categories = new[] { new CategoryView { Id = _category.Id } } };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, _bookToUpdate.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
