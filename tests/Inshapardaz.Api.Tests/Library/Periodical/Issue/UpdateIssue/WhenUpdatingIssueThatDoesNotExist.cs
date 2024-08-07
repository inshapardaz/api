﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssue
{
    [TestFixture]
    public class WhenUpdatingIssueThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueAssert _assert;
        private IssueView _newIssue;
        private PeriodicalDto _periodical;

        public WhenUpdatingIssueThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();

            _newIssue = new IssueView { IssueDate = RandomData.Date };
            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/volumes/1/issues/1", _newIssue);

            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
            _newIssue.VolumeNumber = _newIssue.IssueNumber = 1;
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
        public void ShouldSaveTheChapter()
        {
            _assert.ShouldHaveSavedIssue();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldMatch(_newIssue);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldHaveArticlesLink()
                   .ShouldHavePagesLink()
                   .ShouldHaveCreateArticlesLink()
                   .ShouldHaveCreatePageLink()
                   .ShouldHaveImageUpdateLink();
        }
    }
}
