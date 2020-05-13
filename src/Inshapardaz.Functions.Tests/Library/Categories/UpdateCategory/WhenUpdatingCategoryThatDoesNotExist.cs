﻿using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.UpdateCategory
{
    [TestFixture]
    public class WhenUpdatingCategoryThatDoesNotExist : LibraryTest<Functions.Library.Categories.UpdateCategory>
    {
        private CreatedResult _response;
        private CategoryView _expectedCategory;
        private CategoryAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var faker = new Faker();
            _expectedCategory = new CategoryView { Id = faker.Random.Number(), Name = faker.Random.String() };

            _response = (CreatedResult)await handler.Run(_expectedCategory, LibraryId, _expectedCategory.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
