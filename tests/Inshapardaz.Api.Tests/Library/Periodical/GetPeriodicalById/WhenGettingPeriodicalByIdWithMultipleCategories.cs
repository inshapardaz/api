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
    public class WhenGettingPeriodicalByIdWithMultipleCategories : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalDto _expected;
        private PeriodicalAssert _assert;

        public WhenGettingPeriodicalByIdWithMultipleCategories() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var books = PeriodicalBuilder.WithLibrary(LibraryId)
                                        .WithCategories(4)
                                        .WithIssues(3)
                                        .Build(4);
            _expected = books.PickRandom();

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
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveImageUploadLink()
        {
            _assert.ShouldHaveImageUpdateLink();
        }

        [Test]
        public void ShouldHaveCreateIssueLink()
        {
            _assert.ShouldHaveCreateIssueLink();
        }

        [Test]
        public void ShouldReturnCorrectPeriodicalData()
        {
            _assert.ShouldBeSameAs(_expected, 3, DatabaseConnection);
        }
    }
}
