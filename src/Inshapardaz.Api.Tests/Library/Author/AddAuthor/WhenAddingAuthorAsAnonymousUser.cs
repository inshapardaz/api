using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsAnonymousUser : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = Random.Name };

            _response = await Client.PostObject($"/library/{LibraryId}/authors", author);
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
