using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.DeleteAuthor
{
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    public class WhenDeletingAuthorAsNonAdmin : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingAuthorAsNonAdmin(Permission Permission)
            : base(Permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var authors = AuthorBuilder.WithLibrary(LibraryId).Build(4);
            var expected = authors.PickRandom();

            var client = CreateClient();
            _response = await client.DeleteAsync($"/library/{LibraryId}/authors/{expected.Id}");
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
