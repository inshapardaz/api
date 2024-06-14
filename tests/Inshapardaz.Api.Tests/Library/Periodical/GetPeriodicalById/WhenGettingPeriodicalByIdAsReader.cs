using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicalById
{
    [TestFixture]
    public class WhenGettingPeriodicalByIdAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalDto _expected;
        private PeriodicalAssert _assert;

        public WhenGettingPeriodicalByIdAsReader() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodicals = PeriodicalBuilder.WithLibrary(LibraryId)
                                        .WithIssues(2)
                                        .Build(4);
            _expected = periodicals.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_expected.Id}");
            _assert = PeriodicalAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveImageLink()
        {
            _assert.ShouldHaveImageLink();
        }

        [Test]
        public void ShouldHaveIssuesLink()
        {
            _assert.ShouldHaveIssuesLink();
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                .ShouldNotHaveDeleteLink()
                .ShouldNotHaveImageUpdateLink()
                .ShouldNotHaveCreateIssueLink();
        }

        [Test]
        public void ShouldReturnCorrectPeriodicalData()
        {
            _assert.ShouldBeSameAs(_expected, 2, DatabaseConnection);
        }
    }
}
