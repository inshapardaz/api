using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture]
    public class WhenGettingLibrariesFirstPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesFirstPage()
            : base(Role.Writer, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId, Role.Writer).Build(10);

            _response = await Client.GetAsync($"/libraries?pageNumber=1&pageSize=5");
            _assert = new PagingAssert<LibraryView>(_response);
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
            _assert.ShouldHaveSelfLink("/libraries", 1, 5);
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldNotHaveNextLinks()
        {
            _assert.ShouldHaveNextLink("/libraries", 2, 5);
        }

        [Test]
        public void ShouldReturnExpectedLibraries()
        {
            var expectedItems = LibraryBuilder.Libraries.OrderBy(a => a.Name).Take(5);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatchWithNoConfiguration(item)
                     .ShouldNotHaveEditLinks()
                     .ShouldNotHaveCreateCategoryLink()
                     .ShouldHaveCreateAuthorLink()
                     .ShouldHaveCreateBookLink()
                     .ShouldHaveCreateSeriesLink();
            }
        }
    }
}
