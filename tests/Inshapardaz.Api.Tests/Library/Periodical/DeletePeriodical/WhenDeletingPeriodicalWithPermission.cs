using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.DeletePeriodical
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingPeriodicalWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalAssert _assert;
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
            _assert = Services.GetService<PeriodicalAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveDeletedPeriodical(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedThePriodicalImage()
        {
            _assert.ShouldHaveDeletedPeriodicalImage(_expected.Id);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = CategoryTestRepository.GetCategoriesByBook(_expected.Id);
            var catAssert = Services.GetService<CategoryAssert>().ForLibrary(LibraryId);
            cats.ForEach(cat => catAssert.ShouldNotHaveDeletedCategory(cat.Id));
        }

        [Test]
        public void ShouldBeDeleteIssuesForPeriodical()
        {
            _assert.ShouldHaveDeletedIssuesForPeriodical(_expected.Id);
        }
    }
}
