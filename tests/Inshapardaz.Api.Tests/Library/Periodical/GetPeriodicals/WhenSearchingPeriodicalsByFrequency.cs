using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicals
{
    [TestFixture(PeriodicalFrequency.Annually)]
    [TestFixture(PeriodicalFrequency.Quarterly)]
    [TestFixture(PeriodicalFrequency.Monthly)]
    [TestFixture(PeriodicalFrequency.Fortnightly)]
    [TestFixture(PeriodicalFrequency.Weekly)]
    [TestFixture(PeriodicalFrequency.Daily)]
    public class WhenSearchingPeriodicalsByFrequency : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<PeriodicalView> _assert;
        private IEnumerable<PeriodicalDto> _periodicals;
        private readonly PeriodicalFrequency _frequency;

        public WhenSearchingPeriodicalsByFrequency(PeriodicalFrequency frequency)
            : base(Role.Reader)
        {
            _frequency = frequency;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodicals = PeriodicalBuilder.WithLibrary(LibraryId).Build(30);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals?frequency={_frequency.ToDescription()}");

            _assert = new PagingAssert<PeriodicalView>(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals", 
                new KeyValuePair<string, string> ("frequency", _frequency.ToDescription()));
        }

        [Test]
        public void ShouldReturnExpectedPeriodicals()
        {
            var expectedItems = _periodicals
                    .Where(p => p.Frequency == _frequency)
                    .OrderBy(a => a.Title)
                    .Take(10)
                    .ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, null, DatabaseConnection, LibraryId)
                            .InLibrary(LibraryId);
            };
        }
    }
}
