using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
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
                Language = RandomData.Locale
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_expected.Id}", _expected);
            _periodicalAssert = PeriodicalAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _periodicalAssert.ShouldHaveSavedPeriodical(DatabaseConnection);
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
