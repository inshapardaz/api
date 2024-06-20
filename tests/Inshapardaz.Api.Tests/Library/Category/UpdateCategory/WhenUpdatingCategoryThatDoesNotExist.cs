using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.UpdateCategory
{
    [TestFixture]
    public class WhenUpdatingCategoryThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private CategoryView _expectedCategory;
        private CategoryAssert _assert;

        public WhenUpdatingCategoryThatDoesNotExist()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expectedCategory = new CategoryView { Id = RandomData.Number, Name = RandomData.Name };

            _response = await Client.PutObject($"/libraries/{LibraryId}/categories/{_expectedCategory.Id}", _expectedCategory);
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
