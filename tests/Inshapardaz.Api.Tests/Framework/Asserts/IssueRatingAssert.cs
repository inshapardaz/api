using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using DateTime = System.DateTime;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssueRatingAssert(IIssueTestRepository issueTestRepository)
    {
        private RatingView _view;
        private RatingSummaryView _summary;

        public IssueRatingAssert ForResponse(HttpResponseMessage response)
        {
            _view = response.GetContent<RatingView>().Result;
            return this;
        }

        public IssueRatingAssert ForSummaryResponse(HttpResponseMessage response)
        {
            _summary = response.GetContent<RatingSummaryView>().Result;
            return this;
        }

        public IssueRatingAssert ShouldMatch(int expectedValue)
        {
            _view.Should().NotBeNull();
            _view.Value.Should().Be(expectedValue);
            return this;
        }

        public IssueRatingAssert ShouldHaveAverage(double expectedAverage, int expectedCount)
        {
            _summary.Should().NotBeNull();
            _summary.AverageRating.Should().BeApproximately(expectedAverage, 0.001);
            _summary.TotalCount.Should().Be(expectedCount);
            return this;
        }

        public IssueRatingAssert ShouldHaveSaved(int issueId, int accountId, int expectedValue)
        {
            var rating = issueTestRepository.GetRating(issueId, accountId);

            rating.Should().NotBeNull();
            rating.Value.Should().Be(expectedValue);

            return this;
        }

        public IssueRatingAssert ShouldHaveBeenAdded(int issueId, int accountId)
        {
            var rating = issueTestRepository.GetRating(issueId, accountId);

            rating.Should().NotBeNull();
            rating.DateAdded.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            rating.DateUpdated.Should().BeNull();

            return this;
        }

        public IssueRatingAssert ShouldNotExist(int issueId, int accountId)
        {
            issueTestRepository.GetRating(issueId, accountId).Should().BeNull();
            return this;
        }
    }
}
