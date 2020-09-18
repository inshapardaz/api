using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            book.Title = Random.Name;

            _response = await Client.PutObject($"/library/{LibraryId}/books/{book.Id}", book);
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
