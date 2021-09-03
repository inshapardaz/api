using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture]
    public class WhenGettingLibrariesAsLibraryAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesAsLibraryAdmin()
            : base(Role.LibraryAdmin, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId, Role.LibraryAdmin).Build(4);

            _response = await Client.GetAsync($"/libraries");
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
            var expectedItems = LibraryBuilder.Libraries.OrderBy(a => a.Name).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                     .ShouldHaveUpdateLink()
                     .ShouldNotHaveDeletelink();
            }
        }
    }
}
