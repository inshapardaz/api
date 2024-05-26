using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture]
    public class WhenSearchingLibraries : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;
        private string _startWith = RandomData.String;

        public WhenSearchingLibraries()
            : base(Role.Writer, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId, Role.Writer).StartingWith(_startWith).Build(4);

            _response = await Client.GetAsync($"/libraries?query={_startWith}");
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
            _assert.ShouldHaveSelfLink("/libraries");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedLibraries()
        {
            var expectedItems = LibraryBuilder.Libraries.Where(l => l.Name.StartsWith(_startWith)).OrderBy(a => a.Name).Take(10);
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
