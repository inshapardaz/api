using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.AddBook
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingBookWithPermissions : LibraryTest<Functions.Library.Books.AddBook>
    {
        private readonly ClaimsPrincipal _claim;
        private BookAssert _bookAssert;
        private CreatedResult _response;
        private BooksDataBuilder _builder;
        private BookView _request;

        public WhenAddingBookWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();

            var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();
            var series = Container.GetService<SeriesDataBuilder>().WithLibrary(LibraryId).Build();
            var categories = Container.GetService<CategoriesDataBuilder>().WithLibrary(LibraryId).Build(3);

            _request = Container.GetService<BooksDataBuilder>()
                                    .WithLibrary(LibraryId)
                                    .WithAuthor(author)
                                    .WithSeries(series)
                                    .WithCategory(Random.PickRandom<CategoryDto>(categories))
                                    .WithCategory(Random.PickRandom<CategoryDto>(categories))
                                    .BuildView();

            _response = (CreatedResult)await handler.Run(_request, LibraryId, _claim, CancellationToken.None);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _bookAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheBook()
        {
            _bookAssert.ShouldHaveSavedBook(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _bookAssert.ShouldHaveSelfLink()
                        .ShouldHaveAuthorLink()
                        .ShouldHaveUpdateLink()
                        .ShouldHaveDeleteLink()
                        .ShouldHaveUpdateLink();
        }
    }
}
