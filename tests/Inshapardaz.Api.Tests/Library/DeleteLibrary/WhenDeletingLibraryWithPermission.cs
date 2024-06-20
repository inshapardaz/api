using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.DeleteLibrary
{
    [TestFixture]
    public class WhenDeletingLibraryWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenDeletingLibraryWithPermission()
            : base(Role.Admin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}");
            _assert = Services.GetService<LibraryAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedLibrary()
        {
            _assert.ShouldHaveDeletedLibrary(LibraryId);
        }

        [Test]
        public void ShouldDeleteUnVerifiedOwner()
        {
            Services.GetService<AccountAssert>().AccountShouldNotExist(Library.OwnerEmail);
        }
    }
}
