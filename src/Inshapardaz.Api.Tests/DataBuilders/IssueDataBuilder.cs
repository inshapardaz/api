using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Database.SqlServer;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;
using Inshapardaz.Domain.Models;

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

        internal IEnumerable<IssuePageDto> GetPages(int bookId) => _pages.Where(p => p.IssueId == bookId);

        private bool _hasImage = true;
        private bool? _isPublic = null;
        private int _chapterCount, _contentCount;
        private string _contentMimeType;
        private string _language = null;

        public AuthorDto Author { get; set; }
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;
        private List<AccountItemCountSpec> _favoriteBooks = new List<AccountItemCountSpec>();
        private List<AccountItemCountSpec> _readBooks = new List<AccountItemCountSpec>();
        private List<IssueContentDto> _contents = new List<IssueContentDto>();
        private List<RecentBookDto> _recentBooks = new List<RecentBookDto>();
        private int _pageCount;
        private bool _addPageImage;
        private Dictionary<int, int> _assignments = new Dictionary<int, int>();
        private Dictionary<EditingStatus, int> _pageStatuses = new Dictionary<EditingStatus, int>();

        public IEnumerable<IssueDto> Books => _issues;
        public IEnumerable<IssueContentDto> Contents => _contents;

        public IssueDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
        }

        internal IssueDataBuilder IsPublic(bool isPublic = true)
        {
            _isPublic = isPublic;
            return this;
        }

        public IssueDataBuilder WithChapters(int chapterCount)
        {
            _chapterCount = chapterCount;
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

        public IssueDataBuilder AssignPagesTo(int accountId, int count)
        {
            _assignments.TryAdd(accountId, count);
            return this;
        }

        public IssueDataBuilder WithStatus(EditingStatus statuses, int count)
        {
            _pageStatuses.TryAdd(statuses, count);
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

            _issues = fixture.Build<IssueDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .CreateMany(numberOfIssues)
                          .ToList();

            foreach (var issue in _issues)
            {
                FileDto issueImage = null;
                if (_hasImage)
                {
                    issueImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.BlobUrl)
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
                                         .With(f => f.FilePath, RandomData.BlobUrl)
                                         .With(f => f.IsPublic, false)
                                         .With(f => f.MimeType, mimeType)
                                         .With(f => f.FilePath, RandomData.BlobUrl)
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
                                         .With(a => a.FilePath, RandomData.BlobUrl)
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

                    if (_assignments.Any())
                    {
                        foreach (var assignment in _assignments)
                        {
                            var pagesToAssign = RandomData.PickRandom(pages.Where(p => p.WriterAccountId == null), assignment.Value);
                            foreach (var pageToAssign in pagesToAssign)
                            {
                                pageToAssign.WriterAccountId = assignment.Key;
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
            _connection.DeleteIssuePages(_pages);
            _connection.DeleteIssues(_issues);
            _connection.DeleteFiles(_files);
        }
    }
}
