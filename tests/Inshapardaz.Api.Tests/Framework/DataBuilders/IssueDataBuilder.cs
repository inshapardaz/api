using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using RandomData = Inshapardaz.Api.Tests.Framework.Helpers.RandomData;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{

    public class IssueDataBuilder
    {
        private class AccountItemCountSpec
        {
            public int AccountId { get; set; }
            public int? Count { get; set; }
        }

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
        private Dictionary<EditingStatus, int> _articleStatuses = new Dictionary<EditingStatus, int>();
        private int _articleCount, _tagsCount;
        private int? _year;
        private List<TagDto> _tags = new List<TagDto>();

        private Dictionary<long, List<TagDto>> _issueTags = new();
        private Dictionary<long, List<int>> _articleCategories = new();
        
        public IEnumerable<IssueDto> Issues => _issues;
        public IEnumerable<IssueContentDto> Contents => _contents;
        public IEnumerable<IssueArticleContentDto> ArticleContents => _articleContents;
        public IEnumerable<FileDto> Files => _files;
        
        public Dictionary<long, List<int>> ArticleCategories => _articleCategories;
        public Dictionary<long, List<TagDto>> IssueTags => _issueTags;
        
        private Dictionary<long, IEnumerable<AuthorDto>> _issueArticleAuthors = new ();

        private readonly AuthorsDataBuilder _authorBuilder;
        private readonly TagsDataBuilder _tagsBuilder;

        private readonly ITagTestRepository _tagRepository;
        private IPeriodicalTestRepository _periodicalRepository;
        private IIssueTestRepository _issueRepository;
        private IIssuePageTestRepository _issuePageRepository;
        private IIssueArticleTestRepository _issueArticleRepository;
        private IFileTestRepository _fileRepository;

        public IssueDataBuilder(IFileStorage fileStorage,
               AuthorsDataBuilder authorBuilder,
               TagsDataBuilder tagsBuilder,
               IPeriodicalTestRepository periodicalRepository,
               IFileTestRepository fileRepository,
               IIssueTestRepository issueRepository,
               IIssuePageTestRepository issuePageRepository, 
               IIssueArticleTestRepository issueArticleRepository, 
               ITagTestRepository tagRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _authorBuilder = authorBuilder;
            _tagsBuilder = tagsBuilder;
            _periodicalRepository = periodicalRepository;
            _fileRepository = fileRepository;
            _issueRepository = issueRepository;
            _issuePageRepository = issuePageRepository;
            _issueArticleRepository = issueArticleRepository;
            _tagRepository = tagRepository;
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
        
        public IssueDataBuilder WithTags(int tagsCount)
        {
            _tagsCount = tagsCount;
            return this;
        }
        
        public IssueDataBuilder WithTag(TagDto tag)
        {
            _tags.Add(tag);
            return this;
        }
        
        
        public IssueDataBuilder WithTags(IEnumerable<TagDto> tags)
        {
            _tags = tags.ToList();
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
        
        public IssueDataBuilder WithArticleStatus(EditingStatus statuses, int count)
        {
            if (statuses != EditingStatus.All)
            {
                _articleStatuses.TryAdd(statuses, count);
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

            return fixture.Build<IssueView>()
                .With(b => b.Tags, _tags.Any() ? _tags.Select(c => c.ToView()) : new TagView[0])
                .Create();
        }

        public IssueDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<IssueDto> Build(int numberOfIssues)
        {
            var fixture = new Fixture();

            var isPublic = _isPublic ?? RandomData.Bool;


            if (!_periodicalId.HasValue)
            {
                var periodical = fixture.Build<PeriodicalDto>()
                    .With(b => b.LibraryId, _libraryId)
                    .With(b => b.Language, RandomData.Locale)
                    .Create();

                _periodicalRepository.AddPeriodical(periodical);
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

            IEnumerable<TagDto> tags;
            
            if (_tagsCount > 0 && !_tags.Any())
            {
                tags = _tagsBuilder.WithLibrary(_libraryId).Build(_tagsCount);
            }
            
            else
            {
                tags = _tags;
            }
            
            _issues = fixture.Build<IssueDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .With(b => b.PeriodicalId, _periodicalId)
                          .With(x => x.Status, RandomData.StatusType)
                          .CreateMany(numberOfIssues)
                          .ToList();

            foreach (var issue in _issues)
            {
                if (_hasImage)
                {
                    var issueImage = fixture.Build<FileDto>()
                        .With(a => a.FilePath, RandomData.FilePath)
                        .With(a => a.IsPublic, true)
                        .Create();
                    _fileRepository.AddFile(issueImage);

                    _files.Add(issueImage);
                    _fileStorage.SetupFileContents(issueImage.FilePath, RandomData.Bytes);

                    issue.ImageId = issueImage.Id;
                }
                else
                {
                    issue.ImageId = null;
                }
                
                _issueRepository.AddIssue(issue);


                if (tags != null && tags.Any())
                {
                    _tagRepository.AddIssueToTags(issue.Id, tags);
                    _issueTags.Add(issue.Id, tags.ToList());
                }
                
                List<FileDto> files = null;
                if (_contentCount > 0)
                {
                    var mimeType = MimeTypes.Jpg;
                    var fileName = FilePathHelper.GetPeriodicalIssueImageFileName(RandomData.FileName(mimeType));
                    var filePath = FilePathHelper.GetPeriodicalIssueImageFilePath(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, fileName);
                    files = fixture.Build<FileDto>()
                                         .With(f => f.FilePath, filePath)
                                         .With(f => f.IsPublic, false)
                                         .With(f => f.MimeType, mimeType)
                                         .CreateMany(_contentCount)
                                         .ToList();
                    _files.AddRange(files);
                    files.ForEach(f => _fileStorage.SetupFileContents(f.FilePath, RandomData.Bytes));
                    _fileRepository.AddFiles(files);
                }
                
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
                    _issueRepository.AddIssueFiles(issue.Id, contents);
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
                            var mimeType = MimeTypes.Jpg;
                            var fileName = FilePathHelper.GetIssuePageFileName(RandomData.FileName(mimeType));
                            var filePath = FilePathHelper.GetIssuePageFilePath(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, fileName);

                            pageImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, filePath)
                                         .With(a => a.MimeType, mimeType)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                            _fileRepository.AddFile(pageImage);

                            _files.Add(pageImage);
                            _fileStorage.SetupFileContents(pageImage.FilePath, RandomData.Bytes);
                        }
                        
                        var contentMimeType = MimeTypes.Markdown;
                        var contentFileName = FilePathHelper.IssuePageContentFileName;
                        var contentFilePath = FilePathHelper.GetIssuePageContentPath(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, contentFileName);

                        var contentFile = fixture.Build<FileDto>()
                            .With(a => a.FilePath, contentFilePath)
                            .With(a => a.MimeType, contentMimeType)
                            .With(a => a.IsPublic, true)
                            .Create();
                        _fileRepository.AddFile(contentFile);

                        _files.Add(contentFile);
                        _fileStorage.SetupFileContents(contentFile.FilePath, RandomData.String);
                        

                        pages.Add(fixture.Build<IssuePageDto>()
                            .With(p => p.IssueId, issue.Id)
                            .With(p => p.SequenceNumber, i + 1)
                            .With(p => p.FileId, contentFile.Id)
                            .With(p => p.ImageId, pageImage?.Id)
                            .With(p => p.WriterAccountId, (int?)null)
                            .With(p => p.ReviewerAccountId, (int?)null)
                            .With(p => p.Status, RandomData.EditingStatus )
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
                        var index = 0;
                        foreach (var pageStatus in _pageStatuses)
                        {
                            pages.Skip(index)
                                .Take(pageStatus.Value)
                                .ForEach(p => p.Status = pageStatus.Key);
                            index += pageStatus.Value;
                        }
                    }

                    _issuePageRepository.AddIssuePages(pages);
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
                        var status = RandomData.EditingStatus;
                        if (_articleStatuses.Any())
                        {
                            var top = _articleStatuses.First();
                            status = top.Key;
                            if (top.Value == 1)
                            {
                                _articleStatuses.Remove(top.Key);
                            }
                            else
                            {
                                _articleStatuses[top.Key] = top.Value - 1;
                            }
                        }
                        var article = fixture.Build<IssueArticleDto>()
                            .With(p => p.IssueId, issue.Id)
                            .With(p => p.SequenceNumber, i + 1)
                            .With(p => p.Status, status)
                            .Without(x => x.WriterAccountId)
                            .Without(x => x.WriterAssignTimeStamp)
                            .Without(x => x.ReviewerAccountId)
                            .Without(x => x.ReviewerAssignTimeStamp)
                            .Create();
                        
                        articles.Add(article);
                        _issueArticleRepository.AddIssueArticle(article);

                        if (Author != null)
                        {
                            _issueArticleRepository.AddIssueArticleAuthor(article.Id, Author.Id);
                            _issueArticleAuthors.Add(article.Id, new [] { Author });
                        }
                        else
                        {
                            foreach (var author in _authors)
                            {
                                _issueArticleRepository.AddIssueArticleAuthor(article.Id, author.Id);
                            }

                            _issueArticleAuthors.Add(article.Id, _authors);
                        }

                        if (_articleContentCount > 0)
                        {

                            var articleContents = fixture.Build<IssueArticleContentDto>()
                                    .With(x => x.ArticleId, article.Id)
                                    .With(x => x.Language, () => _articleContentLanguage ?? RandomData.NextLocale())
                                    .CreateMany(_articleContentCount);

                            foreach (var articleContent in articleContents)
                            {
                                var fileName = FilePathHelper.GetIssueArticleContentFileName(articleContent.Language);
                                var filePath = FilePathHelper.GetIssueArticleContentPath(issue.PeriodicalId, issue.VolumeNumber, issue.IssueNumber, article.Id, fileName);
                                var articleContentFile = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, filePath)
                                         .With(a => a.IsPublic, false)
                                         .With(a => a.MimeType, MimeTypes.Markdown)
                                         .Create();
                                _fileRepository.AddFile(articleContentFile);
                                _files.Add(articleContentFile);

                                var articleContentData = RandomData.Text;
                                _fileStorage.SetupFileContents(articleContentFile.FilePath, articleContentData);
                                articleContent.FileId = articleContentFile.Id;
                            }   

                            _issueArticleRepository.AddIssueArticleContents(articleContents);
                            _articleContents.AddRange(articleContents);
                        }
                    }
                    
                    _articles.AddRange(articles);
                }
            }

            return _issues;
        }

        public void CleanUp()
        {
            _issueArticleRepository.DeleteIssueArticles(_articles);
            _issuePageRepository.DeleteIssuePages(_pages);
            _issueRepository.DeleteIssues(_issues);
            _fileRepository.DeleteFiles(_files);
            if (_periodicalId.HasValue) _periodicalRepository.DeletePeriodical(_periodicalId.Value);
        }

        public IEnumerable<AuthorDto> GetAuthorsForIssue(long issueId)
        {
            return _issueArticleAuthors[issueId];
        }
    }
}
