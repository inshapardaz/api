using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture]
    public class WhenGettingChaptersForNonExistingBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            ChapterBuilder.WithLibrary(LibraryId).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{-RandomData.Number}/chapters");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNotFoundResult() => _response.ShouldBeNotFound();
    }
}
