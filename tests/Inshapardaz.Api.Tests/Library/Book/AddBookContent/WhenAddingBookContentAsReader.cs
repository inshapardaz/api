using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.AddBookContent
{
    [TestFixture]
    public class WhenAddingBookContentAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostContent($"/libraries/{LibraryId}/books/{book.Id}/contents?language=pn", RandomData.Bytes, "text/plain");
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbidResult() => _response.ShouldBeForbidden();
    }
}
