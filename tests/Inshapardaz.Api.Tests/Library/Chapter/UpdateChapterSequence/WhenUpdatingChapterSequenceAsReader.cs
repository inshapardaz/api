using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapterSequence
{
    [TestFixture]
    public class WhenUpdatingChapterSequenceAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<ChapterDto> _chapters;

        public WhenUpdatingChapterSequenceAsReader()
            : base(Role.Reader)
        {
        }

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
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
