using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingAuthorAsReader() : base(Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = AuthorBuilder.WithLibrary(LibraryId).Build();

            author.Name = Random.Name;

            _response = await Client.PutObject($"/library/{LibraryId}/authors/{author.Id}", author);
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
