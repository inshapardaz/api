﻿using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.AddPeriodical
{
    [TestFixture]
    public class WhenAddingPeriodicalAsReader : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingPeriodicalAsReader()
            : base(Role.Reader, true)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = new PeriodicalView { Title = RandomData.Name, Description = RandomData.Words(20) };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals", periodical);
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
}
