using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.UpdatePeriodical
{
    [TestFixture]
    public class WhenUpdatingPeriodicalWithAdditionalCategories : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalView _expected;
        private PeriodicalAssert _periodicalAssert;
        private List<CategoryDto> _categoriesToUpdate;

        public WhenUpdatingPeriodicalWithAdditionalCategories()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var periodicals = PeriodicalBuilder.WithLibrary(LibraryId)
                                    .WithCategories(3)
                                    .Build(1);

            var selectedPeriodical = periodicals.PickRandom();

            _categoriesToUpdate = DatabaseConnection.GetCategoriesByPeriodical(selectedPeriodical.Id).ToList();
            _categoriesToUpdate.AddRange(newCategories);

            var fake = new Faker();
            _expected = new PeriodicalView
            {
                Id = selectedPeriodical.Id,
                Title = fake.Name.FullName(),
                Description = fake.Random.Words(5),
                Language = Helpers.RandomData.Locale,
                Categories = _categoriesToUpdate.Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{selectedPeriodical.Id}", _expected);
            _periodicalAssert = PeriodicalAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldReturnCorrectCategories()
        {
            _periodicalAssert.ShouldHaveSameCategories(_categoriesToUpdate);
        }

        [Test]
        public void ShouldSaveCorrectCategories()
        {
            _periodicalAssert.ShouldHaveCategories(_categoriesToUpdate, DatabaseConnection);
        }
    }
}
