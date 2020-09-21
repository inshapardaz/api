using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture]
    public class WhenGettingChaptersForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingChaptersForNonExistingBook()
            : base(Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ChapterBuilder.WithLibrary(LibraryId).Build(4);

            _response = await Client.GetAsync($"/library/{LibraryId}/books/{-Random.Number}/chapters");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
