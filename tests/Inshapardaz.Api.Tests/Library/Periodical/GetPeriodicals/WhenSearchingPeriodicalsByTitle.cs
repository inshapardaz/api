using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicals
{
    [TestFixture]
    public class WhenSearchingPeriodicalsByTitle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<PeriodicalView> _assert;

        public WhenSearchingPeriodicalsByTitle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            PeriodicalBuilder.WithLibrary(LibraryId).Build(30);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals?pageNumber=2&pageSize=10&query=itle");

            _assert = Services.GetService<PagingAssert<PeriodicalView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals");
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals", 3);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/periodicals");
        }

        [Test]
        public void ShouldReturnExpectedPeriodicals()
        {
            var expectedItems = PeriodicalBuilder.Periodicals
                    .Where(b => b.Title.Contains("itle"))
                    .OrderBy(a => a.Title)
                    .Skip(10).Take(10).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<PeriodicalAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected);
            };
        }
    }
}
