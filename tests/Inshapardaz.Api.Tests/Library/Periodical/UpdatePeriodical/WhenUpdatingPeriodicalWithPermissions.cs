using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.UpdatePeriodical
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingPeriodicalWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalView _expected;
        private PeriodicalAssert _periodicalAssert;
        private IEnumerable<CategoryDto> _otherCategories;

        public WhenUpdatingPeriodicalWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAuthor = AuthorBuilder.WithLibrary(LibraryId).Build();
            _otherCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var otherSeries = SeriesBuilder.WithLibrary(LibraryId).Build();
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId)
                                    .WithCategories(3)
                                    .WithIssues(4)
                                    .Build(1);

            var selectedPeriodical = periodical.PickRandom();

            var fake = new Faker();
            _expected = new PeriodicalView
            {
                Id = selectedPeriodical.Id,
                Title = fake.Name.FullName(),
                Description = fake.Random.Words(5),
                Language = RandomData.Locale,
                Frequency = new Faker().PickRandom<PeriodicalFrequency>().ToDescription(),
                Categories = _otherCategories.Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{selectedPeriodical.Id}", _expected);
            _periodicalAssert = Services.GetService<PeriodicalAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedThePeriodical()
        {
            _periodicalAssert.ShouldBeSameAs(_expected, 4);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _periodicalAssert.ShouldHaveSameCategories(_otherCategories);
        }
    }
}
