using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Genre
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenAddingGenreAsReader : IntegrationTestBase
    {
        private GenreView _genreView;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _genreView = new GenreView
            {
                Name = "Some Name"
            };

            Response = await GetReaderClient(Guid.NewGuid()).PostJson($"/api/genres", _genreView);
        }
        
        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}