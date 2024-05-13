﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryById
{
    [TestFixture]
    public class WhenGettingCategoryByIdForCategoryThatDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingCategoryByIdForCategoryThatDoesNotExist()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/{-RandomData.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
