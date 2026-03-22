using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture]
    public class WhenAddingPeriodicalAsReader() : TestBase(Role.Reader, true)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView { Title = RandomData.Name, Description = RandomData.Words(20), Frequency = "Weekly", Language = "en" };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals", periodical);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
