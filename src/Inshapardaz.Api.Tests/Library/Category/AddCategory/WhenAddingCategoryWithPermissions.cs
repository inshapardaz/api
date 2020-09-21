using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.AddCategory
{
    [TestFixture(Domain.Adapters.Permission.Admin)]
    [TestFixture(Domain.Adapters.Permission.LibraryAdmin)]
    public class WhenAddingCategoryWithPermissions : TestBase
    {
        private CategoryView _category;
        private HttpResponseMessage _response;
        private CategoryAssert _assert;

        public WhenAddingCategoryWithPermissions(Domain.Adapters.Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category = new CategoryView { Name = Random.Name };

            _response = await Client.PostObject($"/library/{LibraryId}/categories", _category);
            _assert = CategoryAssert.FromResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveCreatedCategory(DatabaseConnection);
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
