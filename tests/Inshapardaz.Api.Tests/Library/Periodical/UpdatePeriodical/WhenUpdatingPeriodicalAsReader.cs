using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.UpdatePeriodical
{
    [TestFixture]
    public class WhenUpdatingPeriodicalAsReader() : TestBase(Role.Reader)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            periodical.Title = RandomData.Name;

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{periodical.Id}", periodical);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
