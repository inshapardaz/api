﻿using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.AddPage
{
    [TestFixture]
    public class WhenAddingIssuePageForNonExistingPeriodical
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingIssuePageForNonExistingPeriodical()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).Build();

            var page = new IssuePageView { Text = new Faker().Random.String(), SequenceNumber = 1 };
            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{-RandomData.Number}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages", page);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
