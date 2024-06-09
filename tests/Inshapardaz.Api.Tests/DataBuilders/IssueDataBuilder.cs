using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.DataBuilders
{

    public class IssueDataBuilder
    {
        private class AccountItemCountSpec
        {
            public int AccountId { get; set; }
            public int? Count { get; set; }
        }

        private readonly IDbConnection _connection;

        private readonly FakeFileStorage _fileStorage;

        private List<IssueDto> _issues;
        private readonly List<FileDto> _files = new List<FileDto>();
        private List<IssuePageDto> _pages = new List<IssuePageDto>();
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<IssueArticleDto> _articles = new List<IssueArticleDto>();

        internal IEnumerable<IssuePageDto> GetPages(int issuesId) => _pages.Where(p => p.IssueId == issuesId);
        internal IEnumerable<IssueArticleDto> GetArticles(int issuesId) => _articles.Where(a => a.IssueId == issuesId);

        private bool _hasImage = true;
        private bool? _isPublic = null;
        private int _contentCount, _numberOfAuthors, _articleContentCount;
        private string _contentMimeType;
        private string _language = null, _articleContentLanguage;

        public AuthorDto Author { get; set; }
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;
        private List<AccountItemCountSpec> _favoriteBooks = new List<AccountItemCountSpec>();
        private List<AccountItemCountSpec> _readBooks = new List<AccountItemCountSpec>();
        private List<IssueContentDto> _contents = new List<IssueContentDto>();
        private List<IssueArticleContentDto> _articleContents = new List<IssueArticleContentDto>();
        private List<RecentBookDto> _recentBooks = new List<RecentBookDto>();
        private int _pageCount;
        private int? _periodicalId;
        private bool _addPageImage;
        private Dictionary<int, int> _writerAssignments = new Dictionary<int, int>();
        private Dictionary<int, int> _reviewerAssignments = new Dictionary<int, int>();
        private Dictionary<EditingStatus, int> _pageStatuses = new Dictionary<EditingStatus, int>();
        private int _articleCount;
        private int? _year;
        private readonly AuthorsDataBuilder _authorBuilder;

        public IEnumerable<IssueDto> Issues => _issues;
        public IEnumerable<IssueContentDto> Contents => _contents;
        public IEnumerable<IssueArticleContentDto> ArticleContents => _articleContents;

        public IssueDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage, AuthorsDataBuilder authorBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
            _authorBuilder = authorBuilder;
        }

        internal IssueDataBuilder IsPublic(bool isPublic = true)
        {
            _isPublic = isPublic;
            return this;
        }

        internal IssueDataBuilder WithPublishYear(int year)
        {
            _year = year;
            return this;
        }


        public IssueDataBuilder WithNoImage()
        {
            _hasImage = false;
            return this;
        }

        internal IssueDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        internal IssueDataBuilder WithPeriodical(int periodicalId)
        {
            _periodicalId = periodicalId;
            return this;
        }

        public IssueDataBuilder WithContent()
        {
            _contentCount = 1;
            return this;
        }

        internal IssueDataBuilder WithContentLanguage(string language)
        {
            _language = language;
            return this;
        }

        public IssueDataBuilder WithContents(int contentCount, string mimeType = null)
        {
            _contentCount = contentCount;
            _contentMimeType = mimeType;
            return this;
        }

        public IssueDataBuilder WithPages(int count = 10, bool addImage = false)
        {
            _pageCount = count;
            _addPageImage = addImage;
            return this;
        }

        public IssueDataBuilder WithArticles(int count)
        {
            _articleCount = count;
            return this;
        }

        public IssueDataBuilder WithArticleContents(int count)
        {
            _articleContentCount = count;
            return this;
        }
        public IssueDataBuilder WithArticleContentLanguage(string language)
        {
            _articleContentLanguage = language;
            return this;
        }

        public IssueDataBuilder AssignPagesToWriter(int accountId, int count)
        {
            _writerAssignments.TryAdd(accountId, count);
            return this;
        }

        public IssueDataBuilder AssignPagesToReviewer(int accountId, int count)
        {
            _reviewerAssignments.TryAdd(accountId, count);
            return this;
        }

        public IssueDataBuilder WithStatus(EditingStatus statuses, int count)
        {
            if (statuses != EditingStatus.All)
            {
                _pageStatuses.TryAdd(statuses, count);
            }
            return this;
        }

        public IssueDataBuilder WithAuthor(AuthorDto author)
        {
            Author = author;
            return this;
        }

        public IssueDataBuilder WithAuthors(IEnumerable<AuthorDto> authors)
        {
            _authors = authors.ToList();
            return this;
        }

        public IssueDataBuilder WithAuthors(int numberOfAuthors)
        {
            _numberOfAuthors = numberOfAuthors;
            return this;
        }

        public IssueDataBuilder AddToFavorites(int accountId, int? countOfbookToAddToFavorite = null)
        {
            _favoriteBooks.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfbookToAddToFavorite });
            return this;
        }

        public IssueDataBuilder AddToRecentReads(int accountId, int? countOfBookToAddToRecent = null)
        {
            _readBooks.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfBookToAddToRecent });

            return this;
        }

        internal IssueView BuildView()
        {
            var fixture = new Fixture();

            return fixture.Build<IssueView>().Create();
        }

        public IssueDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<IssueDto> Build(int numberOfIssues)
        {
            var fixture = new Fixture();

            Func<bool> isPublic = () => _isPublic ?? RandomData.Bool;


            if (!_periodicalId.HasValue)
            {
                var periodical = fixture.Build<PeriodicalDto>()
                    .With(b => b.LibraryId, _libraryId)
                    .With(b => b.Language, RandomData.Locale)
                    .Create();

                _connection.AddPeriodical(periodical);
                _periodicalId = periodical.Id;
            }

            if (_year.HasValue)
            {
                fixture.Customizations.Add(
                    new RandomDateTimeSequenceGenerator(
                        minDate: new DateTime(_year.Value, 1, 1),
                        maxDate: new DateTime(_year.Value, 12, 31)
                    ));
            }

            _issues = fixture.Build<IssueDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .With(b => b.PeriodicalId, _periodicalId)
                          .CreateMany(numberOfIssues)
                          .ToList();

            foreach (var issue in _issues)
            {
                FileDto issueImage = null;
                if (_hasImage)
                {
                    issueImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(issueImage);

                    _files.Add(issueImage);
                    _fileStorage.SetupFileContents(issueImage.FilePath, RandomData.Bytes);
                    _connection.AddFile(issueImage);

                    issue.ImageId = issueImage.Id;
                }
                else
                {
                    issue.ImageId = null;
                }

                List<FileDto> files = null;
                if (_contentCount > 0)
                {
                    var mimeType = _contentMimeType ?? RandomData.MimeType;
                    files = fixture.Build<FileDto>()
                                         .With(f => f.FilePath, RandomData.FilePath)
                                         .With(f => f.IsPublic, false)
                                         .With(f => f.MimeType, mimeType)
                                         .CreateMany(_contentCount)
                                         .ToList();
                    _files.AddRange(files);
                    files.ForEach(f => _fileStorage.SetupFileContents(f.FilePath, RandomData.Bytes));
                    _connection.AddFiles(files);
                }

                _connection.AddIssue(issue);

                if (files != null)
                {
                    var contents = files.Select(f => new IssueContentDto
                    {
                        IssueId = issue.Id,
                        Language = _language ?? RandomData.NextLocale(),
                        FileId = f.Id,
                        MimeType = f.MimeType,
                        FilePath = f.FilePath
                    }).ToList();
                    _connection.AddIssueFiles(issue.Id, contents);
                    _contents.AddRange(contents);
                }

                if (_pageCount > 0)
                {
                    var pages = new List<IssuePageDto>();

                    for (int i = 0; i < _pageCount; i++)
                    {
                        FileDto pageImage = null;
                        if (_addPageImage)
                        {
                            pageImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                            _connection.AddFile(pageImage);

                            _files.Add(pageImage);
                            _fileStorage.SetupFileContents(pageImage.FilePath, RandomData.Bytes);
                            _connection.AddFile(pageImage);
                        }

                        pages.Add(fixture.Build<IssuePageDto>()
                            .With(p => p.IssueId, issue.Id)
                            .With(p => p.SequenceNumber, i + 1)
                            .With(p => p.Text, RandomData.Text)
                            .With(p => p.ImageId, pageImage?.Id)
                            .With(p => p.WriterAccountId, (int?)null)
                            .With(p => p.ReviewerAccountId, (int?)null)
                            .With(p => p.Status, EditingStatus.All)
                            .Create());
                    }

                    if (_writerAssignments.Any())
                    {
                        foreach (var assignment in _writerAssignments)
                        {
                            var pagesToAssign = RandomData.PickRandom(pages.Where(p => p.WriterAccountId == null && p.ReviewerAccountId == null), assignment.Value);
                            foreach (var pageToAssign in pagesToAssign)
                            {
                                pageToAssign.WriterAccountId = assignment.Key;
                            }
                        }
                    }

                    if (_reviewerAssignments.Any())
                    {
                        foreach (var assignment in _reviewerAssignments)
                        {
                            var pagesToAssign = RandomData.PickRandom(pages.Where(p => p.WriterAccountId == null && p.ReviewerAccountId == null), assignment.Value);
                            foreach (var pageToAssign in pagesToAssign)
                            {
                                pageToAssign.ReviewerAccountId = assignment.Key;
                            }
                        }

                    }

                    if (_pageStatuses.Any())
                    {
                        foreach (var pageStatus in _pageStatuses)
                        {
                            var pagesToSetStatus = RandomData.PickRandom(pages.Where(p => p.Status == EditingStatus.All), pageStatus.Value);
                            foreach (var pageToSetStatus in pagesToSetStatus)
                            {
                                pageToSetStatus.Status = pageStatus.Key;
                            }
                        }
                    }

                    _connection.AddIssuePages(pages);
                    _pages.AddRange(pages);
                }

                if (_articleCount > 0)
                {

                    if (Author == null && !_authors.Any())
                    {
                        _authors = _authorBuilder.WithLibrary(_libraryId).Build(_numberOfAuthors > 0 ? _numberOfAuthors : 1).ToList();
                    }

                    var articles = new List<IssueArticleDto>();

                    for (int i = 0; i < _articleCount; i++)
                    {
                        var article = fixture.Build<IssueArticleDto>()
                            .With(p => p.IssueId, issue.Id)
                            .With(p => p.SequenceNumber, i + 1)
                            .With(p => p.WriterAccountId, (int?)null)
                            .With(p => p.ReviewerAccountId, (int?)null)
                            .With(p => p.Status, EditingStatus.All)
                            .Create();

                        articles.Add(article);
                        _connection.AddIssueArticle(article);

                        if (Author != null)
                        {
                            _connection.AddIssueArticleAuthor(article.Id, Author.Id);
                        }
                        else
                        {
                            foreach (var author in _authors)
                            {
                                _connection.AddIssueArticleAuthor(article.Id, author.Id);
                            }
                        }

                        if (_articleContentCount > 0)
                        {
                            var articleContent = fixture.Build<IssueArticleContentDto>()
                                    .With(x => x.ArticleId, article.Id)
                                    .With(x => x.Language, () => _articleContentLanguage ?? RandomData.String)
                                    .With(x => x.Text, RandomData.String)
                                    .CreateMany(_articleContentCount);

                            _connection.AddIssueArticleContents(articleContent);
                            _articleContents.AddRange(articleContent);
                        }
                    }

                    _articles.AddRange(articles);
                }
            }

            if (_favoriteBooks.Any())
            {
                foreach (var f in _favoriteBooks)
                {
                    var booksToAddToFavorite = f.Count.HasValue ? _issues.PickRandom(f.Count.Value) : _issues;
                    _connection.AddBooksToFavorites(_libraryId, booksToAddToFavorite.Select(b => b.Id), f.AccountId);
                }
            }

            if (_readBooks.Any())
            {
                foreach (var r in _readBooks)
                {
                    var booksToAddToRecent = r.Count.HasValue ? _issues.PickRandom(r.Count.Value) : _issues;
                    foreach (var recentBook in booksToAddToRecent)
                    {
                        RecentBookDto recent = new RecentBookDto { LibraryId = _libraryId, BookId = recentBook.Id, AccountId = r.AccountId, DateRead = RandomData.Date };
                        _connection.AddBookToRecentReads(recent);
                        _recentBooks.Add(recent);
                    }
                }
            }

            return _issues;
        }

        public void CleanUp()
        {
            _connection.DeleteIssueArticles(_articles);
            _connection.DeleteIssuePages(_pages);
            _connection.DeleteIssues(_issues);
            _connection.DeleteFiles(_files);
            if (_periodicalId.HasValue) _connection.DeletePeriodical(_periodicalId.Value);
        }
    }
}
