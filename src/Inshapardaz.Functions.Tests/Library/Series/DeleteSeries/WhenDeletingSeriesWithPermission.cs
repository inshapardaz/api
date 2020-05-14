using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.DeleteSeries
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenDeletingSeriesWithPermission : LibraryTest<Functions.Library.Series.DeleteSeries>
    {
        private NoContentResult _response;

        private SeriesDto _expected;
        private SeriesDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenDeletingSeriesWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<SeriesDataBuilder>();
            var series = _dataBuilder.WithLibrary(LibraryId).WithBooks(3).Build(4);
            _expected = series.First();

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.Id, _claim, CancellationToken.None);
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
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedSeries()
        {
            SeriesAssert.ShouldHaveDeletedSeries(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheSeriesImage()
        {
            SeriesAssert.ShouldHaveDeletedSeriesImage(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldNotDeleteSeriesBooks()
        {
            var seriesBooks = _dataBuilder.Books.Where(b => b.SeriesId == _expected.Id);
            foreach (var book in seriesBooks)
            {
                var b = DatabaseConnection.GetBookById(book.Id);
                b.SeriesId.Should().BeNull();
                b.SeriesIndex.Should().BeNull();
            }
        }
    }
}
