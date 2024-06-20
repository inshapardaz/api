using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private BookView _expected;
        private BookAssert _bookAssert;

        public WhenUpdatingBookThatDoesNotExist() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();

            _expected = new BookView
            {
                Title = RandomData.Name,
                Description = RandomData.Words(10),
                Authors = new List<AuthorView> { new AuthorView { Id = author.Id } },
                Language = RandomData.Locale
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{_expected.Id}", _expected);
            _bookAssert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _bookAssert.ShouldHaveSavedBook();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _bookAssert.ShouldHaveSelfLink()
                        .ShouldHaveUpdateLink()
                        .ShouldHaveDeleteLink()
                        .ShouldHaveUpdateLink();
        }
    }
}
