﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingSeriesWithWritePermissions
        : LibraryTest<Functions.Library.Series.GetSeries>
    {
        private OkObjectResult _response;
        private ListView<SeriesView> _view;
        private readonly ClaimsPrincipal _claim;
        private SeriesDataBuilder _dataBuilder;

        public WhenGettingSeriesWithWritePermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _claim, CancellationToken.None);

            _view = _response.Value as ListView<SeriesView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _view.Links.AssertLink("create")
                 .ShouldBePost()
                 .ShouldHaveSomeHref();
        }
    }
}