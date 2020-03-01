using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeriesById
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingSeriesByIdWithWritePermissions : LibraryTest
    {
        private OkObjectResult _response;
        private SeriesView _view;
        private SeriesDto _selectedSeries;
        private SeriesDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenGettingSeriesByIdWithWritePermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            var series = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _selectedSeries = series.PickRandom();

            var handler = Container.GetService<Functions.Library.Series.GetSeriesById>();
            _response = (OkObjectResult)await handler.Run(request, NullLogger.Instance, LibraryId, _selectedSeries.Id, _claim, CancellationToken.None);

            _view = _response.Value as SeriesView;
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
        public void ShouldReturnCorrectCategoryData()
        {
            Assert.That(_view, Is.Not.Null, "Should contain at-least one series");
            Assert.That(_view.Id, Is.EqualTo(_selectedSeries.Id), "Series id does not match");
            Assert.That(_view.Name, Is.EqualTo(_selectedSeries.Name), "Series name does not match");
            Assert.That(_view.Description, Is.EqualTo(_selectedSeries.Description), "Series description does not match");
            var books = DatabaseConnection.GetBooksBySeries(_selectedSeries.Id);
            Assert.That(_view.BookCount, Is.EqualTo(books.Count()), "Series book count does not match");

            _view.Links.AssertLink(RelTypes.Self)
                 .ShouldBeGet()
                 .ShouldHaveSomeHref();

            _view.Links.AssertLink(RelTypes.Update)
                 .ShouldBePut()
                 .ShouldHaveSomeHref();
            _view.Links.AssertLink(RelTypes.Delete)
                 .ShouldBeDelete()
                 .ShouldHaveSomeHref();
        }
    }
}
