using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
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
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ChapterBuilder.WithLibrary(LibraryId).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{-RandomData.Number}/chapters");
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
