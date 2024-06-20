﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AssignArticleToUser
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignArticleToSelfForReviewing
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private ArticleDto _article;

        public WhenAssignArticleToSelfForReviewing(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder
                .WithLibrary(LibraryId)
                .WithContent()
                .WithStatus(EditingStatus.InReview)
                .Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/ {_article.Id}/assign", new { Type = "review" });
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldAssignArticleToUser()
        {
            _assert
                .ShouldNotBeAssignedForWriting()
                .ShouldBeAssignedToUserForReviewing(Account);
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            _assert
                .ShouldBeSavedNoAssignmentForWriting()
                .ShouldBeSavedAssignmentForReviewing(Account);
        }
    }
}
