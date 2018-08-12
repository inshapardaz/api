using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Category
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenAddingCategoryAsReader : IntegrationTestBase
    {
        private CategoryView _categoryView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categoryView = new CategoryView
            {
                Name = "Some Name"
            };

            Response = await GetReaderClient(Guid.NewGuid()).PostJson($"/api/categories", _categoryView);
        }
        
        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}