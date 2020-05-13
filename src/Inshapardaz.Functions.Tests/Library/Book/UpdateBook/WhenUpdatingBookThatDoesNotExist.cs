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

namespace Inshapardaz.Functions.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookThatDoesNotExist : LibraryTest<Functions.Library.Books.UpdateBook>
    {
        private CreatedResult _response;
        private BookView _expected;
        private BookAssert _bookAssert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = Container.GetService<AuthorsDataBuilder>().WithLibrary(LibraryId).Build();

            var faker = new Faker();
            _expected = new BookView
            {
                Title = faker.Random.String(),
                Description = faker.Random.String(),
                AuthorId = author.Id
            };

            _response = (CreatedResult)await handler.Run(_expected, LibraryId, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
