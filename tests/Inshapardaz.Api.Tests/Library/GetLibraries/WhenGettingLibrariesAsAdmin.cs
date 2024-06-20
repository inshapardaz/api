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
    public class WhenGettingLibrariesAsAdmin : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<LibraryView> _assert;

        public WhenGettingLibrariesAsAdmin()
            : base(Role.Admin, createLibrary: false)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            LibraryBuilder.AssignToUser(AccountId).Build(1);

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
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink("/libraries");
        }

        [Test]
        public void ShouldReturnLibraries()
        {
            _assert.ShouldHaveSomeItems();
        }
    }
}
