using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.DeletePeriodical
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingPeriodicalWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private PeriodicalDto _expected;

        public WhenDeletingPeriodicalWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodicals = PeriodicalBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .WithIssues(3)
                                    .Build(3);

            _expected = periodicals.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedPeriodical()
        {
            PeriodicalAssert.ShouldHaveDeletedPeriodical(DatabaseConnection, _expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedThePriodicalImage()
        {
            PeriodicalAssert.ShouldHaveDeletedPeriodicalImage(DatabaseConnection, _expected.Id);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = DatabaseConnection.GetCategoriesByBook(_expected.Id);
            cats.ForEach(cat => CategoryAssert.ShouldNotHaveDeletedCategory(LibraryId, cat.Id, DatabaseConnection));
        }

        [Test]
        public void ShouldBeDeleteIssuesForPeriodical()
        {
            PeriodicalAssert.ShouldHaveDeletedIssuesForPeriodical(DatabaseConnection, _expected.Id);
        }
    }
}
