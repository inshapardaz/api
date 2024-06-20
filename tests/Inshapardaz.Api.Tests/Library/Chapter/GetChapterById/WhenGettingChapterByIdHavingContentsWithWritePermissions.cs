using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChapterById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingChapterByIdHavingContentsWithWritePermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;
        private ChapterAssert _assert;

        public WhenGettingChapterByIdHavingContentsWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapters = ChapterBuilder.WithLibrary(LibraryId).WithContents(2).Build(4);
            _expected = chapters.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.ChapterNumber}");
            _assert = Services.GetService<ChapterAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
                   .ShouldHaveBookLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldHaveAddChapterContentLink();
        }

        [Test]
        public void ShouldHaveCorrectContents()
        {
            _assert.ShouldHaveCorrectContents();
        }
    }
}
