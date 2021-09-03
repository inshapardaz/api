using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.AddLibrary
{
    [TestFixture(Role.Reader)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingLibraryWithoutPermissions : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingLibraryWithoutPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var library = new LibraryView { Name = RandomData.Name, Language = RandomData.Locale, SupportsPeriodicals = RandomData.Bool };

            _response = await Client.PostObject($"/libraries", library);
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
