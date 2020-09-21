using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.UpdateLibrary
{
    [TestFixture(Permission.Reader)]
    [TestFixture(Permission.Writer)]
    public class WhenUpdatingLibraryWithoutPermission : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingLibraryWithoutPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = new LibraryView { Name = Random.Name, Language = Random.Locale, SupportsPeriodicals = Random.Bool };

            _response = await Client.PutObject($"/library/{LibraryId}", library);
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
