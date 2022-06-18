using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPageById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingBookPageByIdWithWritePermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _expected;
        private BookPageAssert _assert;

        public WhenGettingBookPageByIdWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _expected = BookBuilder.GetPages(book.Id).PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/pages/{_expected.SequenceNumber}");
            _assert = BookPageAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_expected);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldNotHaveImageEditLinks()
        {
            _assert.ShouldNotHaveImageUpdateLink()
                   .ShouldNotHaveImageDeleteLink();
        }
    }
}
