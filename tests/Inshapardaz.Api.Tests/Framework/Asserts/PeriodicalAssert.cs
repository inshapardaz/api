using FluentAssertions;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class PeriodicalAssert(
        IPeriodicalTestRepository periodicalRepository,
        IFileTestRepository fileRepository,
        ICategoryTestRepository categoryRepository,
        FakeFileStorage fileStorage,
        IIssueTestRepository issueRepository,
        ITagTestRepository tagRepository)
    {
        private HttpResponseMessage _response;
        private PeriodicalView _view;
        private int _libraryId;

        private readonly IFileTestRepository _fileRepository = fileRepository;

        public PeriodicalAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _view = response.GetContent<PeriodicalView>().Result;
            return this;
        }

        public PeriodicalAssert ForView(PeriodicalView view)
        {
            _view = view;
            return this;
        }

        public PeriodicalAssert ForLibrary(int libraryId)
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

        public PeriodicalAssert ShouldHaveImageLink()
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

        public PeriodicalAssert ShouldHaveImageLocationHeader()
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        public PeriodicalAssert ShouldHaveSavedPeriodical()
        {
            var dbPeriodical = periodicalRepository.GetPeriodicalById(_view.Id);
            dbPeriodical.Should().NotBeNull();
            _view.Title.Should().Be(dbPeriodical.Title);
            _view.Description.Should().Be(dbPeriodical.Description);
            _view.Language.Should().Be(dbPeriodical.Language);
            _view.Frequency.Should().Be(dbPeriodical.Frequency.ToDescription());
            
            var tags = tagRepository.GetTagsByPeriodical(_view.Id);
            _view.Tags.Select(x => x.Name).Should().BeEquivalentTo(tags.Select(x => x.Name));
            return this;
        }

        public PeriodicalAssert ShouldHaveDeletedPeriodical(int periodicalId)
        {
            var dbPeriodical = periodicalRepository.GetPeriodicalById(periodicalId);
            dbPeriodical.Should().BeNull();
            return this;
        }

        public PeriodicalAssert ShouldHaveDeletedPeriodicalImage(int periodicalId)
        {
            var periodicalImage = periodicalRepository.GetPeriodicalImage(periodicalId);
            periodicalImage.Should().BeNull();
            return this;
        }

        public PeriodicalAssert ShouldHaveDeletedIssuesForPeriodical(int periodicalId)
        {
            var issues = issueRepository.GetIssuesByPeriodical(periodicalId);
            issues.Should().BeNullOrEmpty();
            return this;
        }

        public PeriodicalAssert ShouldBeSameAs(PeriodicalView expected, int? issueCount = null)
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

            var catergories = categoryRepository.GetCategoriesByPeriodical(expected.Id);
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

        public PeriodicalAssert ShouldBeSameAs(PeriodicalDto expected, int? issueCount = null)
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

            var catergories = categoryRepository.GetCategoriesByPeriodical(expected.Id);
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

        public PeriodicalAssert ShouldHaveSameCategories(IEnumerable<CategoryDto> categories)
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
        
        
        public PeriodicalAssert ShouldHaveMatchingTags(IEnumerable<TagView> expectedTags)
        {
            expectedTags.Should().HaveSameCount(_view.Tags);
            foreach (var expectedTag in expectedTags)
            {
                var tag = _view.Tags.SingleOrDefault(c => c.Name == expectedTag.Name);
                tag.Should().NotBeNull();
            }
            return this;
        }


        public PeriodicalAssert ShouldHaveCategories(List<CategoryDto> categories, int? id = null)
        {
            var savedCategories = categoryRepository.GetCategoriesByPeriodical(id ?? _view.Id).Select(c => new { c.Id, c.Name });
            var extectedCategoried = categories.Select(c => new { c.Id, c.Name });

            savedCategories.Should().BeEquivalentTo(extectedCategoried);
            return this;
        }

        public PeriodicalAssert ShouldNotHaveUpdatedPeriodicalImage(int periodicalId, byte[] oldImage)
        {
            var imageUrl = periodicalRepository.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            imageUrl.Should().Contain($"periodicals/{periodicalId}");
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
            return this;
        }

        public PeriodicalAssert ShouldHaveAddedPeriodicalImage(int periodicalId)
        {
            var imageUrl = periodicalRepository.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public PeriodicalAssert ShouldHaveUpdatedPeriodicalImage(int periodicalId, byte[] newImage)
        {
            var imageUrl = periodicalRepository.GetPeriodicalImageUrl(periodicalId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public PeriodicalAssert ShouldHaveChecksum(byte[] contents)
        {
            var file = _response.GetContent<FileView>().Result;
            file.Checksum.Should().Be(ChecksumTestHelper.Compute(contents));
            return this;
        }

        public PeriodicalAssert ShouldHavePublicImage(int periodicalId)
        {
            var image = periodicalRepository.GetPeriodicalImage(periodicalId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }
    }
}
