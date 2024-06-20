using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.GetLibraries
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingLibrariesAs: TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesAs(Role role)
            : base(role, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId, Role.LibraryAdmin).Build(4);

            _response = await Client.GetAsync($"/libraries");
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
            _assert.ShouldHaveSelfLink("/libraries");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }
    }
}
