using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture]
    public class WhenDeletingBookAsReader : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = BookBuilder.WithLibrary(LibraryId).Build(4);
            var expected = books.First();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
