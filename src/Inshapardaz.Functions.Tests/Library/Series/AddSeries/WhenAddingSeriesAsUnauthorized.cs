﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.AddSeries
{
    [TestFixture]
    public class WhenAddingSeriesAsUnauthorized : LibraryTest<Functions.Library.Series.AddSeries>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = new Fixture().Build<SeriesView>().Without(s => s.Links).Without(s => s.BookCount).Create();
            var request = new RequestBuilder()
                                            .WithJsonBody(series)
                                            .Build();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
