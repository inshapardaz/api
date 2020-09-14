using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingAuthorAsReader()
            : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = Random.Name };

            var client = CreateClient();
            _response = await client.PostObject($"/library/{LibraryId}/authors", author);
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
