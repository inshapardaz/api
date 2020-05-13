using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.AddBook
{
    [TestFixture]
    public class WhenAddingBookWithInvalidData
    {
        [TestFixture]
        public class AndUsingNonExistingLibrary : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id };

                _response = (BadRequestResult)await handler.Run(book, -Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingNonExistingAuthor : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var book = new BookView { Title = new Faker().Random.String(), AuthorId = -Random.Number };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingAuthorFromOtherLibrary : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(library2.Id).Build();

                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingNonExistingSeries : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();
                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id, SeriesId = -Random.Number };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingSeriesFromOtherLibrary : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var series = Container.GetService<SeriesDataBuilder>().WithLibrary(library2.Id).Build();
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var book = new BookView { Title = new Faker().Random.String(), AuthorId = author.Id, SeriesId = series.Id };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingNonExistingCategory : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;

            [OneTimeSetUp]
            public async Task Setup()
            {
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();
                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    AuthorId = author.Id,
                    Categories = new CategoryView[] { new CategoryView { Id = -Random.Number } }
                };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
        public class AndUsingCategorysFromOtherLibrary : LibraryTest<Functions.Library.Books.AddBook>
        {
            private BadRequestResult _response;
            private LibraryDataBuilder _library2Builder;

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Container.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();
                var category = Container.GetService<CategoriesDataBuilder>().WithLibrary(library2.Id).Build();
                var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

                var book = new BookView
                {
                    Title = new Faker().Random.String(),
                    AuthorId = author.Id,
                    Categories = new CategoryView[] { new CategoryView { Id = category.Id } }
                };

                _response = (BadRequestResult)await handler.Run(book, LibraryId, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
