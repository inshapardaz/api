using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChapterById
{
    [TestFixture]
    public class WhenGettingChapterByIdAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;
        private ChapterAssert _assert;

        public WhenGettingChapterByIdAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ChapterBuilder.WithLibrary(LibraryId).Build(4).First();

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.Id}");
            _assert = ChapterAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
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
                   .ShouldNotHaveContentsLink();
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink()
                   .ShouldNotHaveAddChapterContentLink();
        }

        [Test]
        public void ShouldNotHaveContentsLink()
        {
            _assert.ShouldHaveNoCorrectContents();
        }
    }
}
