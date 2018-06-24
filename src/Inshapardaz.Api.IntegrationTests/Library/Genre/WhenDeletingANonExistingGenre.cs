using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Genre
{
    [TestFixture]
    public class WhenDeletingANonExistingGenre : IntegrationTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            Response = await GetAdminClient(Guid.NewGuid()).DeleteAsync($"api/genres/{-23243}");
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}