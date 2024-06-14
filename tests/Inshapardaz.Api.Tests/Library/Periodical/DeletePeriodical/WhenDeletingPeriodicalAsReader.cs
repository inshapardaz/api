using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.DeletePeriodical
{
    [TestFixture]
    public class WhenDeletingPeriodicalAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingPeriodicalAsReader() 
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build(4);
            var expected = periodical.First();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{expected.Id}");
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
