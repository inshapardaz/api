﻿using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingPeriodicalWithPermissions
        : TestBase
    {
        private PeriodicalAssert _assert;
        private HttpResponseMessage _response;

        public WhenAddingPeriodicalWithPermissions(Role role)
            : base(role, true)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView 
            { 
                Title = RandomData.Name, 
                Description = RandomData.Words(20),
                Language = RandomData.Locale,
                Frequency = new Faker().PickRandom<PeriodicalFrequency>().ToDescription(),
                Tags = new [] {
                    new TagView { Name = RandomData.Name },
                    new TagView { Name = RandomData.Name }
                }
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals", periodical);

            _assert = Services.GetService<PeriodicalAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldSaveThePeriodical()
        {
            _assert.ShouldHaveSavedPeriodical();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                         .ShouldHaveIssuesLink()
                         .ShouldHaveUpdateLink()
                         .ShouldHaveDeleteLink()
                         .ShouldHaveImageUpdateLink();
        }
    }
}
