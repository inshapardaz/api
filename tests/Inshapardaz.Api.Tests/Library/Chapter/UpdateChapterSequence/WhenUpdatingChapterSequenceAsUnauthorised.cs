using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapterSequence
{
    [TestFixture]
    public class WhenUpdatingChapterSequenceAsUnauthorised
        : TestBase
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
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
