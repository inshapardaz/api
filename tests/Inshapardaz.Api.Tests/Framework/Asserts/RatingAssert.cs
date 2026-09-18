using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using DateTime = System.DateTime;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class RatingAssert(IBookTestRepository bookTestRepository)
    {
        private RatingView _view;
        private RatingSummaryView _summary;

        public RatingAssert ForResponse(HttpResponseMessage response)
        {
            _view = response.GetContent<RatingView>().Result;
            return this;
        }

        public RatingAssert ForSummaryResponse(HttpResponseMessage response)
        {
            _summary = response.GetContent<RatingSummaryView>().Result;
            return this;
        }

        public RatingAssert ShouldMatch(int expectedValue)
        {
            _view.Should().NotBeNull();
            _view.Value.Should().Be(expectedValue);
            return this;
        }

        public RatingAssert ShouldHaveAverage(double expectedAverage, int expectedCount)
        {
            _summary.Should().NotBeNull();
            _summary.AverageRating.Should().BeApproximately(expectedAverage, 0.001);
            _summary.TotalCount.Should().Be(expectedCount);
            return this;
        }

        public RatingAssert ShouldHaveSaved(int bookId, int accountId, int expectedValue)
        {
            var rating = bookTestRepository.GetRating(bookId, accountId);

            rating.Should().NotBeNull();
            rating.Value.Should().Be(expectedValue);

            return this;
        }

        public RatingAssert ShouldHaveBeenAdded(int bookId, int accountId)
        {
            var rating = bookTestRepository.GetRating(bookId, accountId);

            rating.Should().NotBeNull();
            rating.DateAdded.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            rating.DateUpdated.Should().BeNull();

            return this;
        }

        public RatingAssert ShouldHaveBeenUpdated(int bookId, int accountId)
        {
            var rating = bookTestRepository.GetRating(bookId, accountId);

            rating.Should().NotBeNull();
            rating.DateUpdated.Should().NotBeNull();
            rating.DateUpdated.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            return this;
        }

        public RatingAssert ShouldNotExist(int bookId, int accountId)
        {
            bookTestRepository.GetRating(bookId, accountId).Should().BeNull();
            return this;
        }
    }
}
