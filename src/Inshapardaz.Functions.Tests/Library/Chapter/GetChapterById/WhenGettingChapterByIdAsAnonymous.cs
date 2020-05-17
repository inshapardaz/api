using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.GetChapterById
{
    [TestFixture]
    public class WhenGettingChapterByIdAsAnonymous
        : LibraryTest<Functions.Library.Books.Chapters.GetChapterById>
    {
        private OkObjectResult _response;

        private ChapterDto _expected;

        private ChapterAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            _expected = dataBuilder.WithLibrary(LibraryId).WithContents().Build(4).First();

            _response = (OkObjectResult)await handler.Run(LibraryId, _expected.BookId, _expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

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
        public void ShouldSaveTheChapter()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
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
        public void ShouldHaveCorrectContents()
        {
            _assert.ShouldHaveCorrectContents(DatabaseConnection);
        }
    }
}
