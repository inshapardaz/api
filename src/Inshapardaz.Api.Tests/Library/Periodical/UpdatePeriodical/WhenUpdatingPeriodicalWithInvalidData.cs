﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.UpdatePeriodical
{
    [TestFixture]
    public class WhenUpdatingPeriodicalWithInvalidData
    {
        [TestFixture]
        public class AndUsingNonExistingLibrary : TestBase
        {
            private HttpResponseMessage _response;

            public AndUsingNonExistingLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {

                var periodical = new PeriodicalView { Title = RandomData.Name };

                _response = await Client.PutObject($"/libraries/{-RandomData.Number}/periodicals/{periodical.Id}", periodical);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveForbiddenResult()
            {
                _response.ShouldBeForbidden();
            }
        }

        [TestFixture]
        public class AndUpdatingWithNonExistingCategory : TestBase
        {
            private HttpResponseMessage _response;
            private PeriodicalAssert _assert;
            private PeriodicalDto _periodicalToUpdate;
            private CategoryDto _category;

            public AndUpdatingWithNonExistingCategory() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _category = CategoryBuilder.WithLibrary(LibraryId).Build();

                var periodicals = PeriodicalBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _periodicalToUpdate = periodicals.PickRandom();

                var book = new BookView { Title = RandomData.Text, Categories = new[] { new CategoryView { Id = -RandomData.Number } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_periodicalToUpdate.Id}", book);
                _assert = PeriodicalAssert.WithResponse(_response);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }

            [Test]
            public void ShouldNotUpdateTheCategories()
            {
                _assert.ShouldHaveCategories(new List<CategoryDto> { _category }, DatabaseConnection, _periodicalToUpdate.Id);
            }
        }

        [TestFixture]
        public class AndUpdatingWithCategoryFromOtherLibrary : TestBase
        {
            private HttpResponseMessage _response;
            private PeriodicalDto _periodicalToUpdate;
            private CategoryDto _category;
            private LibraryDataBuilder _library2Builder;
            private PeriodicalAssert _assert;

            public AndUpdatingWithCategoryFromOtherLibrary() : base(Role.Writer)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _library2Builder = Services.GetService<LibraryDataBuilder>();
                var library2 = _library2Builder.Build();

                _category = CategoryBuilder.WithLibrary(LibraryId).Build();
                var category = CategoryBuilder.WithLibrary(library2.Id).Build();

                var periodicals = PeriodicalBuilder.WithLibrary(LibraryId).WithCategory(_category).Build(1);
                _periodicalToUpdate = periodicals.PickRandom();

                var periodical = new PeriodicalView { Title = RandomData.Text, Language = RandomData.Locale, Categories = new[] { new CategoryView { Id = category.Id } } };

                _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_periodicalToUpdate.Id}", periodical);
                _assert = PeriodicalAssert.WithResponse(_response);
            }

            [OneTimeTearDown]
            public void Teardown()
            {
                _library2Builder.CleanUp();
                Cleanup();
            }

            [Test]
            public void ShouldHaveBadReqestResult()
            {
                _response.ShouldBeBadRequest();
            }

            [Test]
            public void ShouldNotUpdateTheCategoroes()
            {
                _assert.ShouldHaveCategories(new List<CategoryDto> { _category }, DatabaseConnection, _periodicalToUpdate.Id);
            }
        }
    }
}
