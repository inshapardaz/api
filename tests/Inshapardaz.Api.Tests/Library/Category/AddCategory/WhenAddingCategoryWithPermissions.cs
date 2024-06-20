using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    public class WhenAddingCategoryWithPermissions : TestBase
    {
        private CategoryView _category;
        private HttpResponseMessage _response;
        private CategoryAssert _assert;

        public WhenAddingCategoryWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = new CategoryView { Name = RandomData.Name };

            _response = await Client.PostObject($"/libraries/{LibraryId}/categories", _category);
            _assert = Services.GetService<CategoryAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveCreatedCategoryInDataStore()
        {
            _assert.ShouldHaveCreatedCategory();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                    .ShouldHaveBooksLink()
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink();
        }
    }
}
