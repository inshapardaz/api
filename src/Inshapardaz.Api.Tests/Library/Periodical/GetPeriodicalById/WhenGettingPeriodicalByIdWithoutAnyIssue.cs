﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.GetPeriodicalById
{
    [TestFixture]
    public class WhenGettingPeriodicalByIdWithoutAnyIssue : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalDto _expected;
        private PeriodicalAssert _assert;
        private IEnumerable<CategoryDto> _categories;

        public WhenGettingPeriodicalByIdWithoutAnyIssue() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);
            var periodicals = PeriodicalBuilder.WithLibrary(LibraryId)
                                        .Build(4);
            _expected = periodicals.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_expected.Id}");
            _assert = PeriodicalAssert.WithResponse(_response).InLibrary(LibraryId);
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
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveImageLink()
        {
            _assert.ShouldHaveImageLink();
        }

        [Test]
        public void ShouldHaveIssuesLink()
        {
            _assert.ShouldHaveIssuesLink();
        }

        [Test]
        public void ShouldReturnCorrectPeriodicalData()
        {
            _assert.ShouldBeSameAs(_expected, 0, DatabaseConnection);
        }
    }
}
