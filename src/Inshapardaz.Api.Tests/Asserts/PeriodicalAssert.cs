using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Data;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class PeriodicalAssert
    {
        private HttpResponseMessage _response;
        private PeriodicalView _view;
        private int _libraryId;

        public PeriodicalAssert(HttpResponseMessage response)
        {
            _response = response;
            _view = response.GetContent<PeriodicalView>().Result;
        }

        public static PeriodicalAssert WithResponse(HttpResponseMessage response)
        {
            return new PeriodicalAssert(response);
        }

        public PeriodicalAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public PeriodicalAssert ShouldHaveSelfLink()
        {
            _view.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}");

            return this;
        }

        public PeriodicalAssert ShouldHaveIssuesLink()
        {
            _view.Link("issues")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}/issues");

            return this;
        }

        public PeriodicalAssert ShouldHaveUpdateLink()
        {
            _view.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}");

            return this;
        }

        public PeriodicalAssert ShouldHaveDeleteLink()
        {
            _view.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}");

            return this;
        }

        public PeriodicalAssert ShouldHaveImageUpdateLink()
        {
            _view.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}/image");
            return this;
        }

        public PeriodicalAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/periodicals/{_view.Id}");
            return this;
        }

        internal PeriodicalAssert ShouldHaveSavedPeriodical(IDbConnection dbConnection)
        {
            var dbPeriodical = dbConnection.GetPeriodicalById(_view.Id);
            dbPeriodical.Should().NotBeNull();
            _view.Title.Should().Be(dbPeriodical.Title);
            _view.Description.Should().Be(dbPeriodical.Description);
            return this;
        }
    }
}