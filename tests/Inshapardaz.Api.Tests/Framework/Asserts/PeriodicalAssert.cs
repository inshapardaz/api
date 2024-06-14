using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
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

        public PeriodicalAssert(PeriodicalView view)
        {
            _view = view;
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

        public PeriodicalAssert ShouldHaveCreateIssueLink()
        {
            _view.Link("create-issue")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}/issues");

            return this;
        }

        public PeriodicalAssert ShouldNotHaveCreateIssueLink()
        {
            _view.Link("create-issue").Should().BeNull();

            return this;
        }

        public PeriodicalAssert ShouldHaveUpdateLink()
        {
            _view.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}");

            return this;
        }

        public PeriodicalAssert ShouldNotHaveUpdateLink()
        {
            _view.UpdateLink().Should().BeNull();

            return this;
        }

        public PeriodicalAssert ShouldHaveDeleteLink()
        {
            _view.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}");

            return this;
        }

        public PeriodicalAssert ShouldNotHaveDeleteLink()
        {
            _view.DeleteLink().Should().BeNull();

            return this;
        }

        internal PeriodicalAssert ShouldHaveImageLink()
        {
            _view.Link("image").ShouldBeGet();
            return this;
        }

        public PeriodicalAssert ShouldHaveImageUpdateLink()
        {
            _view.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/periodicals/{_view.Id}/image");
            return this;
        }
        public PeriodicalAssert ShouldNotHaveImageUpdateLink()
        {
            _view.Link("image-upload").Should().BeNull();

            return this;
        }

        public PeriodicalAssert ShouldNotHaveEditLinks()
        {
            return ShouldNotHaveUpdateLink()
                .ShouldNotHaveDeleteLink()
                .ShouldNotHaveImageUpdateLink()
                .ShouldNotHaveCreateIssueLink();
        }

        public PeriodicalAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/periodicals/{_view.Id}");
            return this;
        }

        internal PeriodicalAssert ShouldHaveImageLocationHeader()
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        internal PeriodicalAssert ShouldHaveSavedPeriodical(IDbConnection dbConnection)
        {
            var dbPeriodical = dbConnection.GetPeriodicalById(_view.Id);
            dbPeriodical.Should().NotBeNull();
            _view.Title.Should().Be(dbPeriodical.Title);
            _view.Description.Should().Be(dbPeriodical.Description);
            _view.Language.Should().Be(dbPeriodical.Language);
            _view.Frequency.Should().Be(dbPeriodical.Frequency.ToDescription());
            return this;
        }

        internal static void ShouldHaveDeletedPeriodical(IDbConnection dbConnection, int periodicalId)
        {
            var dbPeriodical = dbConnection.GetPeriodicalById(periodicalId);
            dbPeriodical.Should().BeNull();
        }

        internal static void ShouldHaveDeletedPeriodicalImage(IDbConnection dbConnection, int periodicalId)
        {
            var periodicalImage = dbConnection.GetPeriodicalImage(periodicalId);
            periodicalImage.Should().BeNull();
        }

        internal static void ShouldHaveDeletedIssuesForPeriodical(IDbConnection dbConnection, int periodicalId)
        {
            var issues = dbConnection.GetIssuesByPeriodical(periodicalId);
            issues.Should().BeNullOrEmpty();
        }

        internal PeriodicalAssert ShouldBeSameAs(PeriodicalView expected, int? issueCount, IDbConnection db)
        {
            _view.Should().NotBeNull();
            _view.Title.Should().Be(expected.Title);
            _view.Description.Should().Be(expected.Description);
            _view.Language.Should().Be(expected.Language);
            _view.Frequency.Should().Be(expected.Frequency);
            if (issueCount.HasValue)
            {
                _view.IssueCount.Should().Be(issueCount);
            }

            var catergories = db.GetCategoriesByPeriodical(expected.Id);
            _view.Categories.Should().HaveSameCount(catergories);

            foreach (var catergory in catergories)
            {
                var actual = _view.Categories.SingleOrDefault(a => a.Id == catergory.Id);
                actual.Name.Should().Be(catergory.Name);

                actual.Link("self")
                        .ShouldBeGet()
                        .EndingWith($"libraries/{_libraryId}/categories/{catergory.Id}");
            }

            return this;
        }

        internal PeriodicalAssert ShouldBeSameAs(PeriodicalDto expected, int? issueCount, IDbConnection db)
        {
            _view.Should().NotBeNull();
            _view.Title.Should().Be(expected.Title);
            _view.Description.Should().Be(expected.Description);
            _view.Language.Should().Be(expected.Language);
            _view.Frequency.Should().Be(expected.Frequency.ToDescription());
            if (issueCount.HasValue)
            {
                _view.IssueCount.Should().Be(issueCount);
            }

            var catergories = db.GetCategoriesByPeriodical(expected.Id);
            _view.Categories.Should().HaveSameCount(catergories);

            foreach (var catergory in catergories)
            {
                var actual = _view.Categories.SingleOrDefault(a => a.Id == catergory.Id);
                actual.Name.Should().Be(catergory.Name);

                actual.Link("self")
                        .ShouldBeGet()
                        .EndingWith($"libraries/{_libraryId}/categories/{catergory.Id}");
            }

            return this;
        }

        internal PeriodicalAssert ShouldHaveSameCategories(IEnumerable<CategoryDto> categories)
        {
            foreach (var catergory in categories)
            {
                var actual = _view.Categories.SingleOrDefault(a => a.Id == catergory.Id);
                actual.Name.Should().Be(catergory.Name);

                actual.Link("self")
                        .ShouldBeGet()
                        .EndingWith($"libraries/{_libraryId}/categories/{catergory.Id}");
            }
            return this;
        }

        public PeriodicalAssert ShouldHaveCategories(List<CategoryDto> categories, IDbConnection databaseConnection, int? id = null)
        {
            var savedCategories = databaseConnection.GetCategoriesByPeriodical(id ?? _view.Id).Select(c => new { c.Id, c.Name });
            var extectedCategoried = categories.Select(c => new { c.Id, c.Name });

            savedCategories.Should().BeEquivalentTo(extectedCategoried);
            return this;
        }

        internal static void ShouldNotHaveUpdatedPeriodicalImage(int periodicalId, byte[] oldImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
        }

        internal static void ShouldHaveAddedPeriodicalImage(int periodicalId, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveUpdatedPeriodicalImage(int periodicalId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal static void ShouldHavePublicImage(int periodicalId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetPeriodicalImage(periodicalId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }
    }

    public static class PeriodicalAssertionExtensions
    {
        public static PeriodicalAssert ShouldMatch(this PeriodicalView view, PeriodicalDto dto, int? issueCount, IDbConnection dbConnection, int? libraryId = null)
        {
            if (libraryId.HasValue)
            {
                return new PeriodicalAssert(view)
                            .InLibrary(libraryId.Value)
                           .ShouldBeSameAs(dto, issueCount, dbConnection);
            }
            return new PeriodicalAssert(view)
                               .ShouldBeSameAs(dto, issueCount, dbConnection);
        }
    }
}
