using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.UpdatePeriodical
{
    [TestFixture]
    public class WhenUpdatingPeriodicalThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalView _expected;
        private PeriodicalAssert _periodicalAssert;

        public WhenUpdatingPeriodicalThatDoesNotExist() 
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = new PeriodicalView
            {
                Title = RandomData.Name,
                Description = RandomData.Words(10),
                Language = RandomData.Locale,
                Frequency = new Faker().PickRandom<PeriodicalFrequency>().ToDescription()
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_expected.Id}", _expected);
            _periodicalAssert = Services.GetService<PeriodicalAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _periodicalAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheBook()
        {
            _periodicalAssert.ShouldHaveSavedPeriodical();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _periodicalAssert.ShouldHaveSelfLink()
                        .ShouldHaveUpdateLink()
                        .ShouldHaveDeleteLink()
                        .ShouldHaveUpdateLink();
        }
    }
}
