using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Bogus;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssueArticleAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private IssueDto _issue;
        private IssueArticleView _issueArticle;

        private readonly IIssueArticleTestRepository _issueArticleRepository;
        private readonly IAuthorTestRepository _authorTestRepository;

        public IssueArticleAssert(IIssueArticleTestRepository issueArticleRepository, 
            IAuthorTestRepository authorTestRepository)
        {
            _issueArticleRepository = issueArticleRepository;
            _authorTestRepository = authorTestRepository;
        }

        public IssueArticleAssert ForView(IssueArticleView view)
        {
            _issueArticle = view;
            return this;
        }
        
        public IssueArticleAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public IssueArticleAssert ForDto(IssueDto issue)
        {
            _issue = issue;
            return this;
        }

        public IssueArticleAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _issueArticle = response.GetContent<IssueArticleView>().Result;
            return this;
        }

        public IssueArticleAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}");
            return this;
        }

        public IssueArticleAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _issueArticle.WriterAccountId.Should().Be(account.Id);
            _issueArticle.WriterAccountName.Should().Be(account.Name);
            _issueArticle.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public IssueArticleAssert ShouldNotBeAssignedForWriting()
        {
            _issueArticle.WriterAccountId.Should().BeNull();
            _issueArticle.WriterAccountName.Should().BeNull();
            _issueArticle.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldBeSavedAssignmentForWriting(AccountDto account)
        {
            var dbArticle = _issueArticleRepository.GetIssueArticleById(_issueArticle.Id);
            dbArticle.WriterAccountId.Should().Be(account.Id);
            dbArticle.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public IssueArticleAssert ShouldBeSavedNoAssignmentForWriting()
        {
            var dbArticle = _issueArticleRepository.GetIssueArticleById(_issueArticle.Id);
            dbArticle.WriterAccountId.Should().BeNull();
            dbArticle.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _issueArticle.ReviewerAccountId.Should().Be(account.Id);
            _issueArticle.ReviewerAccountName.Should().Be(account.Name);
            _issueArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public IssueArticleAssert ShouldNotBeAssignedForReviewing()
        {
            _issueArticle.ReviewerAccountId.Should().BeNull();
            _issueArticle.ReviewerAccountName.Should().BeNull();
            _issueArticle.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldBeSavedAssignmentForReviewing(AccountDto account)
        {
            var dbArticle = _issueArticleRepository.GetIssueArticleById(_issueArticle.Id);
            dbArticle.ReviewerAccountId.Should().Be(account.Id);
            dbArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public IssueArticleAssert ShouldBeSavedNoAssignmentForReviewing()
        {
            var dbArticle = _issueArticleRepository.GetIssueArticleById(_issueArticle.Id);
            dbArticle.ReviewerAccountId.Should().BeNull();
            dbArticle.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveSavedArticle()
        {
            var dbArticle = _issueArticleRepository.GetIssueArticleById(_issueArticle.Id);
            dbArticle.Should().NotBeNull();
            _issueArticle.Title.Should().Be(dbArticle.Title);
            _issueArticle.SeriesName.Should().Be(dbArticle.SeriesName);
            _issueArticle.SeriesIndex.Should().Be(dbArticle.SeriesIndex);
            _issueArticle.WriterAccountId.Should().Be(dbArticle.WriterAccountId);
            if (dbArticle.WriterAssignTimeStamp.HasValue)
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeCloseTo(dbArticle.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeNull();
            }
            _issueArticle.ReviewerAccountId.Should().Be(dbArticle.ReviewerAccountId);
            if (dbArticle.ReviewerAssignTimeStamp.HasValue)
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(dbArticle.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().BeNull();
            }
            _issueArticle.Status.Should().Be(dbArticle.Status.ToDescription());
            _issueArticle.SequenceNumber.Should().Be(dbArticle.SequenceNumber );

            var dbAuthors = _authorTestRepository.GetAuthorsByIssueArticle(dbArticle.Id).ToArray();
            _issueArticle.Authors.Should().HaveSameCount(dbAuthors);
            foreach (var issueAuthor in _issueArticle.Authors)
            {
                var dbAuthor = dbAuthors.SingleOrDefault(x => x.Id == issueAuthor.Id);
                dbAuthor.Should().NotBeNull();
                dbAuthor.Id.Should().Be(issueAuthor.Id);
                dbAuthor.Name.Should().Be(issueAuthor.Name);
            }
            
            return this;
        }

        public IssueArticleAssert ShouldHaveDeletedArticle(int articleId)
        {
            var article = _issueArticleRepository.GetIssueArticleById(articleId);
            article.Should().BeNull();
            return this;
        }

        public IssueArticleAssert ThatContentsAreDeletedForArticle(int articleId)
        {
            var contents = _issueArticleRepository.GetContentByIssueArticle(articleId);
            contents.Should().BeNullOrEmpty();
            return this;
        }

        public IssueArticleAssert ShouldHaveSelfLink()
        {
            _issueArticle.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}");

            return this;
        }

        public IssueArticleAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink()
            .ShouldHavePeriodicalLink()
            .ShouldHaveIssueLink()
            .ShouldNotHaveAddArticleContentLink()
            .ShouldNotHaveUpdateLink()
            .ShouldNotHaveDeleteLink()
            .ShouldNotHaveAssignmentLink();
            return this;
        }

        public IssueArticleAssert WithWriteableLinks()
        {
            ShouldHaveAddIssueContentLink()
            .ShouldHaveUpdateLink()
            .ShouldHaveDeleteLink()
            .ShouldHaveAssignmentLink();
            return this;
        }

        public IssueArticleAssert ShouldHaveContentLink(IssueArticleContentDto content)
        {
            var actual = _issueArticle.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
            return this;
        }

        public IssueArticleAssert ShouldHaveNoCorrectContents()
        {
            _issueArticle.Link("content").Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveIssueLink()
        {
            _issueArticle.Link("issue")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}");

            return this;
        }

        public IssueArticleAssert ShouldHavePeriodicalLink()
        {
            _issueArticle.Link("periodical")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}");

            return this;
        }

        public IssueArticleAssert ShouldHaveAssignmentLink()
        {
            _issueArticle.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}/assign");
            return this;
        }

        public IssueArticleAssert ShouldNotHaveAssignmentLink()
        {
            _issueArticle.Link("assign")
                  .Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveCorrectContents()
        {
            var contents = _issueArticleRepository.GetIssueArticleContents(_issueArticle.Id);

            contents.Should().HaveSameCount(_issueArticle.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }
            return this;
        }

        public IssueArticleAssert ShouldHaveUpdateLink()
        {
            _issueArticle.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}");

            return this;
        }

        public IssueArticleAssert ShouldNotHaveUpdateLink()
        {
            _issueArticle.UpdateLink().Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveDeleteLink()
        {
            _issueArticle.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}");

            return this;
        }

        public IssueArticleAssert ShouldNotHaveDeleteLink()
        {
            _issueArticle.DeleteLink().Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveAddIssueContentLink()
        {
            _issueArticle.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}/contents");

            return this;
        }

        public IssueArticleAssert ShouldNotHaveAddArticleContentLink()
        {
            _issueArticle.Link("add-content").Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveUpdateArticleContentLink(IssueContentDto content)
        {
            var actual = _issueArticle.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        public IssueArticleAssert ShouldHaveDeleteIssueContentLink(IssueContentDto content)
        {
            var actual = _issueArticle.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_issueArticle.SequenceNumber}/contents")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        public IssueArticleAssert ShouldNotHaveContentsLink()
        {
            _issueArticle.Link("content").Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveNotNextLink()
        {
            _issueArticle.Link("next").Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHaveNextLink(int sequenceNumber)
        {
            _issueArticle.Link("next")
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{sequenceNumber}");
            return this;
        }

        public IssueArticleAssert ShouldHaveNotPreviousLink()
        {
            _issueArticle.Link("previous").Should().BeNull();
            return this;
        }

        public IssueArticleAssert ShouldHavePreviousLink(int sequenceNumber)
        {
            _issueArticle.Link("previous")
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{sequenceNumber}");
            return this;
        }

        public IssueArticleAssert ShouldMatch(IssueArticleView view)
        {
            _issueArticle.Title.Should().Be(view.Title);
            _issueArticle.SequenceNumber.Should().Be(view.SequenceNumber);
            _issueArticle.WriterAccountId.Should().Be(view.WriterAccountId);
            _issueArticle.WriterAccountName.Should().Be(view.WriterAccountName);
            if (view.WriterAssignTimeStamp.HasValue)
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.WriterAssignTimeStamp.Should().Be(view.WriterAssignTimeStamp);
            }
            _issueArticle.ReviewerAccountId.Should().Be(view.ReviewerAccountId);
            if (view.ReviewerAssignTimeStamp.HasValue)
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().Be(view.ReviewerAssignTimeStamp);
            }
            _issueArticle.ReviewerAccountName.Should().Be(view.ReviewerAccountName);
            _issueArticle.SeriesName.Should().Be(view.SeriesName);
            _issueArticle.SeriesIndex.Should().Be(view.SeriesIndex);
            _issueArticle.Status.Should().Be(view.Status);
            return this;
        }

        public IssueArticleAssert ShouldMatch(IssueArticleDto dto, IEnumerable<AuthorDto> authors)
        {
            _issueArticle.Title.Should().Be(dto.Title);
            _issueArticle.SequenceNumber.Should().Be(dto.SequenceNumber);
            _issueArticle.WriterAccountId.Should().Be(dto.WriterAccountId);
            if (dto.WriterAssignTimeStamp.HasValue)
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.WriterAssignTimeStamp.Should().Be(dto.WriterAssignTimeStamp);
            }

            _issueArticle.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimeStamp.HasValue)
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().Be(dto.ReviewerAssignTimeStamp);

            }

            _issueArticle.SeriesName.Should().Be(dto.SeriesName);
            _issueArticle.SeriesIndex.Should().Be(dto.SeriesIndex);
            _issueArticle.Status.Should().Be(dto.Status.ToString());

            _issueArticle.Authors.Should().HaveSameCount(authors);

            foreach (var author in _issueArticle.Authors)
            {
                var expectedAuthor = authors.SingleOrDefault(x => x.Id == author.Id);
                author.Name.Should().Be(expectedAuthor.Name);
            }
            
            return this;
        }

        public IssueArticleAssert ShouldBeSameAs(IssueArticleDto dto)
        {
            _issueArticle.Title.Should().Be(dto.Title);
            _issueArticle.SequenceNumber.Should().Be(dto.SequenceNumber);
            _issueArticle.WriterAccountId.Should().Be(dto.WriterAccountId);
            if (dto.WriterAssignTimeStamp.HasValue)
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.WriterAssignTimeStamp.Should().BeNull();
            }
            _issueArticle.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimeStamp.HasValue)
            {
                _issueArticle.ReviewerAssignTimeStamp.Should()
                    .BeCloseTo(dto.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issueArticle.ReviewerAssignTimeStamp.Should().BeNull();
            }
            _issueArticle.SeriesName.Should().Be(dto.SeriesName);
            _issueArticle.SeriesIndex.Should().Be(dto.SeriesIndex);
            _issueArticle.Status.Should().Be(dto.Status.ToString());

            return this;
        }
    }
}
