using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapterSequence
{
    [TestFixture]
    public class WhenUpdatingChapterSequenceAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;
        private IEnumerable<ChapterDto> _chapters;

        [OneTimeSetUp]
        public async Task Setup()
        {

            _chapters = ChapterBuilder.WithLibrary(LibraryId).Public().Build(3);
            var bookId = _chapters.First().BookId;

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{bookId}/chapters/sequence", _chapters);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
