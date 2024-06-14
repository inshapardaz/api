using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AssignArticleToUser
{
    [TestFixture]
    public class WhenAssignArticleToReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;

        public WhenAssignArticleToReader()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var reader = AccountBuilder.InLibrary(LibraryId).As(Role.Reader).Build();
            _article = ArticleBuilder.WithLibrary(LibraryId).WithContent().Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/{_article.Id}/assign", new { AccountId = reader.Id, Type = "write" });
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
