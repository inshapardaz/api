﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.AddIssueContent
{
    [TestFixture]
    public class WhenAddingIssueContentForNonExistingPeriodical
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingIssueContentForNonExistingPeriodical()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostContent($"/libraries/{LibraryId}/periodicals/{-RandomData.Number}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents?language=pn", RandomData.Bytes, "text/plain");
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
