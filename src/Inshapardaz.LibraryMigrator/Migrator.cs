using System.Text.Json;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Storage.FileSystem;
using Inshapardaz.Storage.S3;
using ShellProgressBar;

namespace Inshapardaz.LibraryMigrator;

public class Migrator
{
    RepositoryFactory SourceRepositoryFactory { get; }
    RepositoryFactory DestinationRepositoryFactory { get; }

    private readonly bool _writeTextToDatabase = true;
    IFileStorage _fileStore;

    public Migrator(string source, DatabaseTypes sourceType, string destination, DatabaseTypes destinationType)
    {
        SourceRepositoryFactory = new RepositoryFactory(source, sourceType);
        DestinationRepositoryFactory = new RepositoryFactory(destination, destinationType);
    }

    public async Task Migrate(int libraryId, bool correctionsOnly, bool production, CancellationToken cancellationToken)
    {
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            ProgressBarOnBottom = true,
            EnableTaskBarProgress = true
        };
        const int totalTicks = 10;
        
        if (correctionsOnly)
        {
            using var pBar1 = new ProgressBar(totalTicks, "Migration started", options);
            
            Console.WriteLine("Migration of correction started.");

            await MigrateCorrections(pBar1, cancellationToken);

            await MigrateAllAccounts(pBar1, cancellationToken);

            Console.WriteLine("Migration of correction finished.");

            return;
        }

        using var pBar = new ProgressBar(totalTicks, "Migration started", options);
        var sourceLibraryDb = SourceRepositoryFactory.LibraryRepository;
        var destinationLibraryDb = DestinationRepositoryFactory.LibraryRepository;

        pBar.Tick("Step 1 of 10 - Migrating Library");
        var sourceLibrary = await sourceLibraryDb.GetLibraryById(libraryId, cancellationToken);

        sourceLibrary.DatabaseConnection = string.Empty;
        if (sourceLibrary.ImageId.HasValue)
        {
            var libraryImage = await CopyFile(sourceLibrary.ImageId.Value, cancellationToken);
            sourceLibrary.ImageId = libraryImage.Id;
        }

        var newLibrary = await destinationLibraryDb.AddLibrary(sourceLibrary, cancellationToken);

        var config = JsonSerializer.Deserialize<S3Configuration>(newLibrary.FileStoreSource);
        _fileStore =  production ? new S3FileStorage(config) : new FileSystemStorage("../FileStore");

        pBar.Tick("Step 2 of 10 - Migrating Accounts");

        var accountsMap = await MigrateLibraryAccounts(newLibrary.Id, pBar, cancellationToken);

        pBar.Tick("Step 3 of 10 - Migrating Authors");
        var authorMap = await MigrateAuthors(libraryId, newLibrary.Id, pBar, cancellationToken);

        pBar.Tick("Step 4 of 10 - Migrating Series");
        var seriesMap = await MigrateSeries(libraryId, newLibrary.Id, pBar, cancellationToken);

        pBar.Tick("Step 5 of 10 - Migrating Categories");
        var categoriesMap = await MigrateCategories(libraryId, newLibrary.Id, pBar, cancellationToken);

        pBar.Tick("Step 6 of 10 - Migrating Books");
        var booksMap = await MigrateBooks(libraryId, newLibrary.Id, authorMap, seriesMap, categoriesMap, accountsMap, pBar, cancellationToken);

        pBar.Tick("Step 7 of 10 - Migrating Periodicals");
        await MigratePeriodicals(libraryId, newLibrary.Id, authorMap, categoriesMap, accountsMap, pBar, cancellationToken);

        pBar.Tick("Step 8 of 10 - Migrating BookShelves");
        await MigrateBookShelves(libraryId, newLibrary.Id, accountsMap, booksMap, pBar, cancellationToken);

        pBar.Tick("Step 9 of 10 - Migrating Articles");
        await MigrateArticles(libraryId, newLibrary.Id, authorMap, categoriesMap, pBar, cancellationToken);

        pBar.Tick("Step 10 of 10 - Migration Completed");
    }

    private async Task MigrateCorrections(ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.CorrectionRepository;
        var destinationDb = DestinationRepositoryFactory.CorrectionRepository;

        var corrections = await sourceDb.GetAllCorrections(cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        var correctionModels = corrections as CorrectionModel[] ?? corrections.ToArray();
        using var child = pBar.Spawn(correctionModels.Count(), "Migrating correction", options);
        Console.WriteLine($"Migrating {correctionModels.Count()} correction.");
        int i = 0;

        foreach (var correction in correctionModels)
        {
            await destinationDb.AddCorrection(correction, cancellationToken);
            child.Tick($"Step {++i} of {correctionModels.Count()} - Correction(s) migrated.");
        }
    }

    private async Task MigrateAllAccounts(ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.AccountRepository;
        var destinationDb = DestinationRepositoryFactory.AccountRepository;

        var accounts = await sourceDb.GetAccounts(1, int.MaxValue, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)accounts.TotalCount, "Migrating Accounts", options);
        int i = 0;
        foreach (var account in accounts.Data)
        {
            var existingAccount = await destinationDb.GetAccountByEmail(account.Email, cancellationToken);

            if (existingAccount == null)
            {
                await destinationDb.AddAccount(account, cancellationToken);
            }

            child.Tick($"Step {++i} of {accounts.TotalCount} - Account(s) migrated.");
        }
    }

    private async Task<Dictionary<int, int>> MigrateLibraryAccounts(int newLibraryId, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.AccountRepository;
        var destinationDb = DestinationRepositoryFactory.AccountRepository;

        Dictionary<int, int> accountsMap = new Dictionary<int, int>();
        var accounts = await sourceDb.GetAccounts(1, int.MaxValue, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)accounts.TotalCount, "Migrating Accounts", options);
        int i = 0;
        foreach (var account in accounts.Data)
        {
            var oldAccountId = account.Id;
            var existingAccount = await destinationDb.GetAccountByEmail(account.Email, cancellationToken);
            if (existingAccount == null)
            {
                existingAccount = await destinationDb.AddAccount(account, cancellationToken);
            }

            await destinationDb.AddAccountToLibrary(newLibraryId, existingAccount.Id, existingAccount.Role, cancellationToken);
            accountsMap.Add(oldAccountId, existingAccount.Id);

            child.Tick($"Step {++i} of {accounts.TotalCount} - Account(s) migrated.");
        }

        return accountsMap;
    }

    private async Task<Dictionary<int, int>> MigrateAuthors(int libraryId, int newLibraryId, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.AuthorRepository;
        var destinationDb = DestinationRepositoryFactory.AuthorRepository;

        Dictionary<int, int> authorMap = new Dictionary<int, int>();
        var authors = await sourceDb.GetAuthors(libraryId, null, 1, int.MaxValue, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)authors.TotalCount, "Migrating Authors", options);
        int i = 0;

        foreach (var author in authors.Data)
        {
            if (author.ImageId.HasValue)
            {
                var authorImage = await CopyFile(author.ImageId.Value, cancellationToken);
                author.ImageId = authorImage.Id;
            }

            var newAuthor = await destinationDb.AddAuthor(newLibraryId, author, cancellationToken);
            child.Tick($"Step {++i} of {authors.TotalCount} - Author(s) migrated.");

            authorMap.Add(author.Id, newAuthor.Id);
        }

        return authorMap;
    }

    private async Task<Dictionary<int, int>> MigrateSeries(int libraryId, int newLibraryId, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.SeriesRepository;
        var destinationDb = DestinationRepositoryFactory.SeriesRepository;

        Dictionary<int, int> seriesMap = new Dictionary<int, int>();
        var series = await sourceDb.GetSeries(libraryId, 1, int.MaxValue, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)series.TotalCount, "Migrating Series", options);
        int i = 0;

        foreach (var s in series.Data)
        {
            if (s.ImageId.HasValue)
            {
                var seriesImage = await CopyFile(s.ImageId.Value, cancellationToken);
                s.ImageId = seriesImage.Id;
            }

            var newSeries = await destinationDb.AddSeries(newLibraryId, s, cancellationToken);
            child.Tick($"{++i} of {series.TotalCount} Series(s) migrated.");

            seriesMap.Add(s.Id, newSeries.Id);
        }

        return seriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateCategories(int libraryId, int newLibraryId, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.CategoryRepository;
        var destinationDb = DestinationRepositoryFactory.CategoryRepository;

        Dictionary<int, int> categoriesMap = new Dictionary<int, int>();
        var categories = (await sourceDb.GetCategories(libraryId, cancellationToken)).ToArray();

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn(categories.Length, "Migrating Series", options);
        int i = 0;

        foreach (var category in categories)
        {
            var newCategory = await destinationDb.AddCategory(newLibraryId, category, cancellationToken);
            child.Tick($"{++i} of {categories.Length} Categories migrated.");
            categoriesMap.Add(category.Id, newCategory.Id);
        }

        return categoriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateBooks(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> seriesMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookRepository;
        var destinationDb = DestinationRepositoryFactory.BookRepository;

        Dictionary<int, int> booksMap = new Dictionary<int, int>();

        var pageNumber = 1;
        var bookPage = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using var child = pBar.Spawn((int)bookPage.TotalCount, "Migrating Books", options);
        int i = 0;

        do
        {
            foreach (var book in bookPage.Data)
            {
                if (book.ImageId.HasValue)
                {
                    var bookImage = await CopyFile(book.ImageId.Value, cancellationToken);
                    book.ImageId = bookImage.Id;
                }

                book.Authors = book.Authors
                    .Select(a => new AuthorModel { Id = authorMap[a.Id] })
                    .ToList();

                book.Categories = book.Categories
                    .Select(c => new CategoryModel { Id = categoriesMap[c.Id] })
                    .ToList();

                book.SeriesId = book.SeriesId.HasValue ? seriesMap[book.SeriesId.Value] : null;

                var newBook = await destinationDb.AddBook(newLibraryId, book, null, cancellationToken);
                booksMap.Add(book.Id, newBook.Id);

                var chaptersMap = await MigrateChapters(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, child, cancellationToken);
                await MigrateBookPages(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, chaptersMap, child, cancellationToken);

                var bookContents = await sourceDb.GetBookContents(libraryId, book.Id, cancellationToken);
                foreach (var content in bookContents)
                {
                    var bookContent = await CopyFile(content.FileId, cancellationToken);
                    content.BookId = newBook.Id;
                    content.FileId = bookContent.Id;

                    await destinationDb.AddBookContent(newLibraryId, bookContent.Id, content.Language, cancellationToken);
                }

                child.Tick($"{++i} of {bookPage.TotalCount} Book(s) migrated.");

            }

            bookPage = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);
        }
        while (bookPage.Data.Any());

        return booksMap;
    }

    private async Task<Dictionary<long, long>> MigrateChapters(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, ChildProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ChapterRepository;
        var destinationDb = DestinationRepositoryFactory.ChapterRepository;

        var chaptersMap = new Dictionary<long, long>();
        var chapters = (await sourceDb.GetChaptersByBook(libraryId, bookId, cancellationToken)).ToArray();
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn(chapters.Length, "Migrating Chapters for book " + newBookId, options);
        int i = 0;

        foreach (var chapter in chapters)
        {
            chapter.BookId = newBookId;
            if (chapter.ReviewerAccountId.HasValue)
            {
                if (accountsMap.TryGetValue(chapter.ReviewerAccountId.Value, out var value))
                {
                    chapter.ReviewerAccountId = value;
                }
            }
            else
            {
                chapter.ReviewerAccountId = null;
            }

            if (chapter.WriterAccountId.HasValue)
            {
                if (accountsMap.TryGetValue(chapter.WriterAccountId.Value, out var value))
                {
                    chapter.WriterAccountId = value;
                }
                else
                {
                    chapter.WriterAccountId = null;
                }
            }
            else
            {
                chapter.WriterAccountId = null;
            }

            var newChapter = await destinationDb.AddChapter(newLibraryId, newBookId, chapter, cancellationToken);

            chaptersMap.Add(chapter.Id, newChapter.Id);

            var contents = await sourceDb.GetChapterContents(libraryId, bookId, chapter.ChapterNumber, cancellationToken);

            foreach (var content in contents)
            {
                content.BookId = newBookId;
                content.ChapterId = newChapter.Id;

                if (!_writeTextToDatabase)
                {
                    var fileName = FilePathHelper.BookChapterContentFileName;
                    var filePath = await _fileStore.StoreTextFile(FilePathHelper.GetBookChapterContentPath(bookId, fileName), content.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.FileId = file.Id;
                    content.Text = string.Empty;
                }
                    
                await destinationDb.AddChapterContent(newLibraryId, content, cancellationToken);
            }

            child.Tick($"{++i} of {chapters.Length} Chapters(s) migrated for book {newBookId}.");
        }

        return chaptersMap;
    }

    private async Task MigrateBookPages(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, Dictionary<long, long> chaptersMap, ChildProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookPageRepository;
        var destinationDb = DestinationRepositoryFactory.BookPageRepository;

        var pageNumber = 1;
        var pagesPage = await sourceDb.GetPagesByBook(libraryId, bookId, pageNumber++, 10, EditingStatus.All,
            AssignmentFilter.All, AssignmentFilter.All, null, cancellationToken); 
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using var child = pBar.Spawn((int)pagesPage.TotalCount, "Migrating Pages for book " + newBookId, options);
        int i = 0;
        do
        {
            foreach (var page in pagesPage.Data)
            {
                page.BookId = newBookId;
                page.ChapterId = page.ChapterId.HasValue ? chaptersMap[page.ChapterId.Value] : null;

                if (page.ImageId.HasValue)
                {
                    var pageImage = await CopyFile(page.ImageId.Value, cancellationToken);
                    page.ImageId = pageImage.Id;
                }

                if (page.ReviewerAccountId.HasValue)
                {
                    if (accountsMap.TryGetValue(page.ReviewerAccountId.Value, out var value))
                    {
                        page.ReviewerAccountId = value;
                        page.ReviewerAssignTimeStamp = page.ReviewerAssignTimeStamp;
                    }
                    else
                    {
                        page.ReviewerAccountId = null;
                    }
                }
                else
                {
                    page.ReviewerAccountId = null;
                }

                if (page.WriterAccountId.HasValue)
                {
                    if (accountsMap.TryGetValue(page.WriterAccountId.Value, out var value))
                    {
                        page.WriterAccountId = value;
                        page.WriterAssignTimeStamp = page.WriterAssignTimeStamp;
                    }
                    else
                    {
                        page.WriterAccountId = null;
                    }
                }
                else
                {
                    page.WriterAccountId = null;
                }

                if (!_writeTextToDatabase)
                {
                    var fileName = FilePathHelper.BookPageContentFileName;
                    var filePath = await _fileStore.StoreTextFile(FilePathHelper.GetBookPageContentPath(bookId, fileName), page.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    page.ContentId = file.Id;
                    page.Text = string.Empty;
                }

                await destinationDb.AddPage(newLibraryId, page, cancellationToken);

                child.Tick($"{++i} of {pagesPage.TotalCount} Pages(s) migrated for book {newBookId}.");
            }
            
            pagesPage = await sourceDb.GetPagesByBook(libraryId, bookId, pageNumber++, 10, EditingStatus.All,
                AssignmentFilter.All, AssignmentFilter.All, null, cancellationToken);
        }
        while (pagesPage.Data.Any());
    }

    private async Task MigratePeriodicals(int libraryId, int newLibraryId, Dictionary<int, int> authorMap,
        Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, ProgressBar pBar,
        CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.PeriodicalRepository;
        var destinationDb = DestinationRepositoryFactory.PeriodicalRepository;

        var periodicals = await sourceDb.GetPeriodicals(libraryId, null, 1, int.MaxValue, new PeriodicalFilter(), PeriodicalSortByType.DateCreated, SortDirection.Ascending, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)periodicals.TotalCount, "Migrating Periodicals", options);
        int i = 0;

        foreach (var periodical in periodicals.Data)
        {
            if (periodical.ImageId.HasValue)
            {
                var periodicalImage = await CopyFile(periodical.ImageId.Value, cancellationToken);
                periodical.ImageId = periodicalImage.Id;
            }

            periodical.Categories = periodical.Categories
                .Select(c => new CategoryModel { Id = categoriesMap[c.Id] })
                .ToList();

            var newPeriodical = await destinationDb.AddPeriodical(newLibraryId, periodical, cancellationToken);

            await MigrateIssue(libraryId, newLibraryId, periodical.Id, newPeriodical.Id, authorMap, accountsMap, child, cancellationToken);
                
            child.Tick($"{++i} of {periodicals.TotalCount} Periodical(s) migrated.");
        }
    }

    private async Task MigrateIssue(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId,
        Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, ChildProgressBar pBar,
        CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.IssueRepository;
        var destinationDb = DestinationRepositoryFactory.IssueRepository;

        var issues = await sourceDb.GetIssues(libraryId, periodicalId, 1, int.MaxValue, new IssueFilter(), IssueSortByType.VolumeNumberAndIssueNumber, SortDirection.Ascending, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using var child = pBar.Spawn((int)issues.TotalCount, "Migrating Issues for periodical " + newPeriodicalId, options);
        int i = 0;

        foreach (var issue in issues.Data)
        {
            if (issue.ImageId.HasValue)
            {
                var issueImage = await CopyFile(issue.ImageId.Value, cancellationToken);
                issue.ImageId = issueImage.Id;
            }

            var newIssue = await destinationDb.AddIssue(newLibraryId, newPeriodicalId, issue, cancellationToken);

            var contents = await sourceDb.GetIssueContents(libraryId, periodicalId, issue.VolumeNumber, issue.IssueNumber, cancellationToken);

            foreach (var content in contents)
            {
                content.PeriodicalId = newPeriodicalId;

                var newFile = await CopyFile(content.FileId, cancellationToken);
                await destinationDb.AddIssueContent(newLibraryId,
                    new IssueContentModel
                    {
                        PeriodicalId = issue.PeriodicalId,
                        VolumeNumber = issue.VolumeNumber,
                        IssueNumber = issue.IssueNumber,
                        FileId = newFile.Id,
                        Language = content.Language,
                        MimeType = content.MimeType,
                    },
                    cancellationToken);
            }

            //TODO:  migrate pages and move to files from content
            await MigrateIssueArticle(libraryId, newLibraryId, periodicalId, newPeriodicalId, newIssue.Id, newIssue.VolumeNumber, newIssue.IssueNumber, authorMap, accountsMap, child, cancellationToken);

            child.Tick($"{++i} of {issues.TotalCount} Issues(s) migrated for periodical {periodicalId}.");
        }
    }

    private async Task MigrateIssueArticle(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId,
        int newIssueId, int volumeNumber, int issueNumber, Dictionary<int, int> authorMap,
        Dictionary<int, int> accountsMap, ChildProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.IssueArticleRepository;
        var destinationDb = DestinationRepositoryFactory.IssueArticleRepository;

        var articles = await sourceDb.GetArticlesByIssue(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        var issueArticleModels = articles as IssueArticleModel[] ?? articles.ToArray();
        using var child = pBar.Spawn(issueArticleModels.Count(), $"Migrating Articles for periodical {newPeriodicalId} volume {volumeNumber} issue {issueNumber}", options);
        int i = 0;
        foreach (var article in issueArticleModels)
        {
            article.Authors = article.Authors
                .Select(a => new AuthorModel { Id = authorMap[a.Id] })
                .ToList();

            article.IssueId = newIssueId;

            article.ReviewerAccountId = article.ReviewerAccountId.HasValue ? accountsMap[article.ReviewerAccountId.Value] : null;
            article.WriterAccountId = article.WriterAccountId.HasValue ? accountsMap[article.WriterAccountId.Value] : null;

            var newArticle = await destinationDb.AddArticle(newLibraryId, newPeriodicalId, volumeNumber, issueNumber, article, cancellationToken);

            var contents = await sourceDb.GetArticleContents(libraryId, periodicalId, volumeNumber, issueNumber, article.SequenceNumber, cancellationToken);

            foreach (var content in contents)
            {
                content.PeriodicalId = periodicalId;

                if (!_writeTextToDatabase)
                {
                    var fileName = FilePathHelper.GetIssueArticleContentFileName(content.Language);
                    var filePath = await _fileStore.StoreTextFile(FilePathHelper.GetIssueArticleContentPath(periodicalId, volumeNumber, issueNumber, newArticle.Id, fileName), content.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.FileId = file.Id;
                    content.Text = string.Empty;
                }

                await destinationDb.AddArticleContent(
                    newLibraryId,
                    new IssueArticleContentModel
                    {
                        PeriodicalId = content.PeriodicalId,
                        VolumeNumber = content.VolumeNumber,
                        IssueNumber = content.IssueNumber,
                        SequenceNumber = content.SequenceNumber,
                        Language = content.Language,
                        Text = content.Text,
                        FileId = content.FileId,
                    },
                    cancellationToken);
            }

            child.Tick($"{++i} of {issueArticleModels.Count()} Issues(s) migrated for periodical {newPeriodicalId} volume {volumeNumber} issue {issueNumber}.");
        }
    }

    private async Task MigrateBookShelves(int libraryId, int newLibraryId, Dictionary<int, int> accountsMap,
        Dictionary<int, int> booksMap, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookShelfRepository;
        var destinationDb = DestinationRepositoryFactory.BookShelfRepository;

        var bookshelves = await sourceDb.GetAllBookShelves(libraryId, 1, int.MaxValue, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using var child = pBar.Spawn((int)bookshelves.TotalCount, "Migrating Book shelves", options);
        int i = 0;

        foreach (var bookShelf in bookshelves.Data)
        {
            bookShelf.AccountId = accountsMap[bookShelf.AccountId];
            if (bookShelf.ImageId.HasValue)
            {
                var bookShelfImage = await CopyFile(bookShelf.ImageId.Value, cancellationToken);
                bookShelf.ImageId = bookShelfImage.Id;
            }

            var newBookshelf = await destinationDb.AddBookShelf(newLibraryId, bookShelf, cancellationToken);

            var books = await sourceDb.GetBookShelfBooks(libraryId, bookShelf.Id, cancellationToken);

            foreach (var book in books)
            {
                await destinationDb.AddBookToBookShelf(newLibraryId, newBookshelf.Id, booksMap[book.BookId], book.Index, cancellationToken);
            }

            child.Tick($"{++i} of {bookshelves.TotalCount} Book shelve(s) migrated.");

        }
    }

    private async Task MigrateArticles(int libraryId, int newLibraryId, Dictionary<int, int> authorMap,
        Dictionary<int, int> categoriesMap, ProgressBar pBar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ArticleRepository;
        var destinationDb = DestinationRepositoryFactory.ArticleRepository;
        
        var articles = await sourceDb.GetAllArticles(libraryId, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        var articleModels = articles as ArticleModel[] ?? articles.ToArray();
        using var child = pBar.Spawn(articleModels.Length, "Migrating Articles", options);
        int i = 0;

        foreach (var article in articleModels)
        {
            if (article.ImageId.HasValue)
            {
                var articleImage = await CopyFile(article.ImageId.Value, cancellationToken);
                article.ImageId = articleImage.Id;
            }

            article.Authors = article.Authors
                .Select(a => new AuthorModel { Id = authorMap[a.Id] })
                .ToList();

            article.Categories = article.Categories
                .Select(c => new CategoryModel { Id = categoriesMap[c.Id] })
                .ToList();

            var newArticle = await destinationDb.AddArticle(newLibraryId, article, null, cancellationToken);

            foreach (var content in article.Contents)
            {
                content.ArticleId = newArticle.Id;

                if (!_writeTextToDatabase)
                {
                    var fileName = FilePathHelper.GetArticleContentFileName(content.Language);
                    var filePath = await _fileStore.StoreTextFile(
                        FilePathHelper.GetArticleContentPath(article.Id, fileName), content.Text,
                        cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.FileId = file.Id;
                    content.Text = string.Empty;
                }

                await destinationDb.AddArticleContent(newLibraryId, content, cancellationToken);
            }

            child.Tick($"{++i} of {articleModels.Count()} article(s) migrated.");

        }
    }

    private async Task<FileModel> AddFile(string fileName, string filePath, string mimeType, CancellationToken cancellationToken)
    {
        var destinationDb = DestinationRepositoryFactory.FileRepository;

        return await destinationDb.AddFile(new FileModel
        {
            FileName = fileName,
            FilePath = filePath,
            MimeType = mimeType,
            DateCreated = DateTime.Now,
            IsPublic = false
        }, cancellationToken);
    }

    private async Task<FileModel> CopyFile(long fileId, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.FileRepository;
        var destinationDb = DestinationRepositoryFactory.FileRepository;

        var file = await sourceDb.GetFileById(fileId, cancellationToken);
        return await destinationDb.AddFile(file, cancellationToken);
    }

}
