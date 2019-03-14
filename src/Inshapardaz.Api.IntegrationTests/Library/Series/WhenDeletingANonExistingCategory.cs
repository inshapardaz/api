﻿using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Series
{
    [TestFixture, Ignore("Role based security not implemented")]
    public class WhenDeletingANonExistingCategory : IntegrationTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            Response = await GetAdminClient(Guid.NewGuid()).DeleteAsync($"api/series/{-23243}");
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}