using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Author
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenAddingAuthorAsReader : IntegrationTestBase
    {
        private AuthorView _genreView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genreView = new AuthorView
            {
                Name = "Some Name"
            };

            Response = await GetReaderClient(Guid.NewGuid()).PostJson($"/api/authors", _genreView);
        }
        
        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}