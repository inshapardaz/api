using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture]
    public class WhenGettingLibrariesLastPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesLastPage()
            : base(Role.Writer, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId, Role.Writer).Build(15);

            _response = await Client.GetAsync($"/libraries?pageNumber=3&pageSize=5");
            _assert = Services.GetService<PagingAssert<LibraryView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink("/libraries", 3, 5);
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink("/libraries", 2, 5);
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldReturnExpectedLibraries()
        {
            var expectedItems = LibraryBuilder.Libraries.OrderBy(a => a.Name).Skip(10).Take(5);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<LibraryAssert>().ForView(actual).ForLibrary(actual.Id)
                    .ShouldBeSameWithNoConfiguration(item)
                    .ShouldNotHaveEditLinks()
                    .ShouldNotHaveCreateCategoryLink()
                    .ShouldHaveCreateAuthorLink()
                    .ShouldHaveCreateBookLink()
                    .ShouldHaveCreateSeriesLink();
            }
        }
    }
}
