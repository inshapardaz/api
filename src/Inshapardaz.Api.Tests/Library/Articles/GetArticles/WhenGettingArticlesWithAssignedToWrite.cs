﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticles
{
    [TestFixture]
    public class WhenGettingArticlesWithAssignedToWrite : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<ArticleView> _assert;

        public WhenGettingArticlesWithAssignedToWrite() 
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ArticleBuilder.WithLibrary(LibraryId)
                .WithCategories(2)
                .WithContents(2)
                .WithWtiterAssignment(AccountId)
                .Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles?assignedfor=writer");

            _assert = new PagingAssert<ArticleView>(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/articles", 
                new KeyValuePair<string, string>("assignedfor", "writer"));
        }

        [Test]
        public void ShouldReturnExpectedArticles()
        {
            var expectedItems = ArticleBuilder.Articles
                .Where(x => x.WriterAccountId == AccountId)
                .OrderBy(a => a.Title).Take(10).ToArray();

            _assert.Data.Count().Should().Be(expectedItems.Length);
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                actual.ShouldMatch(expected, DatabaseConnection, LibraryId)
                            .ShouldHaveSelfLink()
                            .WithWriteableLinks()
                            .ShouldHaveImageUpdateLink()
                            .ShouldHaveAddContentLink()
                            .ShouldHavePublicImageLink()
                            .ShouldHaveContents(ArticleBuilder.Contents.Where(c => c.ArticleId == actual.Id).ToList(), true)
                            .ShouldBeSameAs(expected, DatabaseConnection);
            }
        }
    }
}
