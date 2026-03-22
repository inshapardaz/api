using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models.Library;
using DateTime = System.DateTime;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class ReadProgressAssert(IBookTestRepository bookTestRepository)
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private ReadProgressView _view;

        public ReadProgressAssert ForView(ReadProgressView view)
        {
            _view = view;
            return this;
        }

        public ReadProgressAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _view = response.GetContent<ReadProgressView>().Result;
            return this;
        }

        public ReadProgressAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ReadProgressAssert ShouldHaveSaved(int bookId, int accountId, ReadProgressView view)
        {
            var progress = bookTestRepository.GetBookProgress(bookId, accountId);
        
            progress.Should().NotBeNull();

            progress.ProgressType.Should().Be(view.ProgressType.ToEnum(ProgressType.Unknown));
            progress.ProgressId.Should().Be(view.ProgressId);
            progress.ProgressValue.Should().Be(view.ProgressValue);
            progress.DateRead.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
            return this;
        }

        public void ShouldMatch(ReadProgressView view)
        {
            _view.ProgressId.Should().Be(view.ProgressId);
            _view.ProgressType.Should().Be(view.ProgressType);
            _view.ProgressValue.Should().Be(view.ProgressValue);
        }
    }
}
