using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Storage.FileSystem;
using ShellProgressBar;

namespace Inshapardaz.LibraryMigrator;

public class Migrator
{
    RepositoryFactory SourceRepositoryFactory { get; init; }
    RepositoryFactory DestinationRepositoryFactory { get; init; }

    public Migrator(string source, DatabaseTypes sourceType, string destination, DatabaseTypes destinationType)
    {
        SourceRepositoryFactory = new RepositoryFactory(source, sourceType);
        DestinationRepositoryFactory = new RepositoryFactory(destination, destinationType);
    }

    public async Task Migrate(int libraryId, bool correctionsOnly, CancellationToken cancellationToken)
    {
        if (correctionsOnly)
        {
            Console.WriteLine("Migration of correction started.");

            await MigrateCorrections(cancellationToken);

            Console.WriteLine("Migration of correction finished.");

            return;
        }

        const int totalTicks = 10;

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            ProgressBarOnBottom = true,
            EnableTaskBarProgress = true
        };

        using (var pbar = new ProgressBar(totalTicks, "Migration started", options))
        {

            Dictionary<int, int> _authorMap;
            Dictionary<int, int> _seriesMap;
            Dictionary<int, int> _categoriesMap;
            Dictionary<int, int> _booksMap;
            Dictionary<int, int> _periodicalsMap;
            Dictionary<int, int> _accountsMap;

            var sourceLibraryDb = SourceRepositoryFactory.LibraryRepository;
            var destinationLibraryDb = DestinationRepositoryFactory.LibraryRepository;

            pbar.Tick("Step 1 of 10 - Migrating Library");
            var sourceLibrary = await sourceLibraryDb.GetLibraryById(libraryId, cancellationToken);

            sourceLibrary.DatabaseConnection = string.Empty;
            if (sourceLibrary.ImageId.HasValue)
            {
                var libraryImage = await CopyFile(sourceLibrary.ImageId.Value, cancellationToken);
                sourceLibrary.ImageId = libraryImage.Id;
            }


            var newLibrary = await destinationLibraryDb.AddLibrary(sourceLibrary, cancellationToken);

            pbar.Tick("Step 2 of 10 - Migrating Accounts");

            _accountsMap = await MigrateAccounts(libraryId, newLibrary.Id, pbar, cancellationToken);

            pbar.Tick("Step 3 of 10 - Migrating Authors");
            _authorMap = await MigrateAuthors(libraryId, newLibrary.Id, pbar, cancellationToken);

            pbar.Tick("Step 4 of 10 - Migrating Series");
            _seriesMap = await MigrateSeries(libraryId, newLibrary.Id, pbar, cancellationToken);

            pbar.Tick("Step 5 of 10 - Migrating Categories"); 
            _categoriesMap = await MigrateCategories(libraryId, newLibrary.Id, pbar, cancellationToken);

            pbar.Tick("Step 6 of 10 - Migrating Books");
            _booksMap = await MigrateBooks(libraryId, newLibrary.Id, _authorMap, _seriesMap, _categoriesMap, _accountsMap, pbar, cancellationToken);

            pbar.Tick("Step 7 of 10 - Migrating Periodicals");
            _periodicalsMap = await MigratePeriodicals(libraryId, newLibrary.Id, _authorMap, _categoriesMap, _accountsMap, pbar, cancellationToken);

            pbar.Tick("Step 8 of 10 - Migrating BookShelves");
            var bookshelfCount = await MigrateBookShelves(libraryId, newLibrary.Id, _accountsMap, _booksMap, pbar, cancellationToken);

            pbar.Tick("Step 9 of 10 - Migrating Articles");
            await MigrateArticles(libraryId, newLibrary.Id, _accountsMap, _authorMap, _categoriesMap, pbar, cancellationToken);

            pbar.Tick("Step 10 of 10 - Migration Completed");
        }
    }

    public async Task MigrateCorrections(CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.CorrectionRepository;
        var destinationDb = DestinationRepositoryFactory.CorrectionRepository;

        var corrections = await sourceDb.GetAllCorrections(cancellationToken);

        Console.WriteLine($"Migrating {corrections.Count()} correction.");

        foreach (var correction in corrections)
        {
            await destinationDb.AddCorrection(correction, cancellationToken);
        }
    }

    private async Task<Dictionary<int, int>> MigrateAccounts(int libraryId, int newLibraryId, ProgressBar pbar, CancellationToken cancellationToken)
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

        using (var child = pbar.Spawn((int)accounts.TotalCount, "Migrating Accounts", options))
        {
            int i = 0;
            foreach (var account in accounts.Data)
            {
                var existingAccount = await destinationDb.GetAccountByEmail(account.Email, cancellationToken);

                if (existingAccount == null)
                {
                    existingAccount = await destinationDb.AddAccount(account, cancellationToken);
                }

                await destinationDb.AddAccountToLibrary(newLibraryId, existingAccount.Id, account.Role, cancellationToken);

                child.Tick($"Step {++i} of {accounts.TotalCount} - Account(s) migrated.");

                accountsMap.Add(account.Id, existingAccount.Id);
            }

        }
        return accountsMap;
    }

    private async Task<Dictionary<int, int>> MigrateAuthors(int libraryId, int newLibraryId, ProgressBar pbar, CancellationToken cancellationToken)
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

        using (var child = pbar.Spawn((int)authors.TotalCount, "Migrating Authors", options))
        {

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
        }
        return authorMap;
    }

    private async Task<Dictionary<int, int>> MigrateSeries(int libraryId, int newLibraryId, ProgressBar pbar, CancellationToken cancellationToken)
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

        using (var child = pbar.Spawn((int)series.TotalCount, "Migrating Series", options))
        {
            int i = 0;

            foreach (var serie in series.Data)
            {
                if (serie.ImageId.HasValue)
                {
                    var serieImage = await CopyFile(serie.ImageId.Value, cancellationToken);
                    serie.ImageId = serieImage.Id;
                }

                var newSerie = await destinationDb.AddSeries(newLibraryId, serie, cancellationToken);
                child.Tick($"{++i} of {series.TotalCount} Series(s) migrated.");

                seriesMap.Add(serie.Id, newSerie.Id);
            }

        }

        return seriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateCategories(int libraryId, int newLibraryId, ProgressBar pbar, CancellationToken cancellationToken)
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

        using (var child = pbar.Spawn(categories.Length, "Migrating Series", options))
        {
            int i = 0;

            foreach (var category in categories)
            {
                var newCategory = await destinationDb.AddCategory(newLibraryId, category, cancellationToken);
                child.Tick($"{++i} of {categories.Length} Categories migrated.");
                categoriesMap.Add(category.Id, newCategory.Id);
            }
        }

        return categoriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateBooks(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> seriesMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, ProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookRepository;
        var destinationDb = DestinationRepositoryFactory.BookRepository;

        Dictionary<int, int> booksMap = new Dictionary<int, int>();

        var pageNumber = 1;
        var page = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using (var child = pbar.Spawn((int)page.TotalCount, "Migrating Books", options))
        {
            int i = 0;

            do
            {
                foreach (var book in page.Data)
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

                    foreach (var content in book.Contents)
                    {
                        var bookContent = await CopyFile(content.FileId, cancellationToken);
                        content.BookId = newBook.Id;
                        content.FileId = bookContent.Id;

                        await destinationDb.AddBookContent(newLibraryId, bookContent.Id, content.Language, cancellationToken);
                    }

                    child.Tick($"{++i} of {page.TotalCount} Book(s) migrated.");

                }

                page = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);
            }
            while (page.Data.Count() > 0);
        }

        return booksMap;
    }

    private async Task<Dictionary<long, long>> MigrateChapters(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, ChildProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ChapterRepository;
        var destinationDb = DestinationRepositoryFactory.ChapterRepository;
        var fileStore = new FileSystemStorage("../FileStore");

        var chaptersMap = new Dictionary<long, long>();
        var chapters = (await sourceDb.GetChaptersByBook(libraryId, bookId, cancellationToken)).ToArray();
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using (var child = pbar.Spawn(chapters.Length, "Migrating Chapters for book " + newBookId, options))
        {
            int i = 0;

            foreach (var chapter in chapters)
            {
                chapter.BookId = newBookId;
                if (chapter.ReviewerAccountId.HasValue && accountsMap.ContainsKey(chapter.ReviewerAccountId.Value))
                {
                    chapter.ReviewerAccountId = accountsMap[chapter.ReviewerAccountId.Value];
                }
                else
                {
                    chapter.ReviewerAccountId = null;
                }

                if (chapter.WriterAccountId.HasValue && accountsMap.ContainsKey(chapter.WriterAccountId.Value))
                {
                    chapter.WriterAccountId = accountsMap[chapter.WriterAccountId.Value];
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

                    var fileName = FilePathHelper.BookChapterContentFileName;
                    var filePath = await fileStore.StoreTextFile(FilePathHelper.GetBookChapterContentPath(bookId, fileName), content.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.Text = string.Empty;
                    content.FileId = file.Id;
                    await destinationDb.AddChapterContent(newLibraryId, content, cancellationToken);
                }

                child.Tick($"{++i} of {chapters.Length} Chapters(s) migrated for book {newBookId}.");
            }

        }

        return chaptersMap;
    }

    private async Task MigrateBookPages(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, Dictionary<long, long> chaptersMap, ChildProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookPageRepository;
        var destinationDb = DestinationRepositoryFactory.BookPageRepository;
        var fileStore = new FileSystemStorage("../FileStore");

        var pages = (await sourceDb.GetAllPagesByBook(libraryId, bookId, cancellationToken)).ToArray();
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using (var child = pbar.Spawn(pages.Length, "Migrating Pages for book " + newBookId, options))
        {
            
            int i = 0;

            foreach (var page in pages)
            {
                page.BookId = newBookId;
                page.ChapterId = page.ChapterId.HasValue ? chaptersMap[page.ChapterId.Value] : null;

                if (page.ImageId.HasValue)
                {
                    var pageImage = await CopyFile(page.ImageId.Value, cancellationToken);
                    page.ImageId = pageImage.Id;
                }

                if (page.ReviewerAccountId.HasValue && accountsMap.ContainsKey(page.ReviewerAccountId.Value))
                {
                    page.ReviewerAccountId = accountsMap[page.ReviewerAccountId.Value];
                    page.ReviewerAssignTimeStamp = page.ReviewerAssignTimeStamp;
                }
                else
                {
                    page.ReviewerAccountId = null;
                }

                if (page.WriterAccountId.HasValue && accountsMap.ContainsKey(page.WriterAccountId.Value))
                {
                    page.WriterAccountId = accountsMap[page.WriterAccountId.Value];
                    page.WriterAssignTimeStamp = page.WriterAssignTimeStamp;
                }
                else
                {
                    page.WriterAccountId = null;
                }

                var fileName = FilePathHelper.BookPageContentFileName;
                var filePath = await fileStore.StoreTextFile(FilePathHelper.GetBookContentPath(bookId, fileName), page.Text, cancellationToken);
                var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                page.Text = string.Empty;
                page.ContentId = file.Id;

                await destinationDb.AddPage(newLibraryId, page, cancellationToken);

                child.Tick($"{++i} of {pages.Length} Pages(s) migrated for book {newBookId}.");
            }
        }
    }

    private async Task<Dictionary<int, int>> MigratePeriodicals(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, ProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.PeriodicalRepository;
        var destinationDb = DestinationRepositoryFactory.PeriodicalRepository;
        var periodicalMap = new Dictionary<int, int>();

        var periodicals = await sourceDb.GetPeriodicals(libraryId, null, 1, int.MaxValue, new PeriodicalFilter(), PeriodicalSortByType.DateCreated, SortDirection.Ascending, cancellationToken);

        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using (var child = pbar.Spawn((int)periodicals.TotalCount, "Migrating Periodicals", options))
        {

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

                periodicalMap.Add(periodical.Id, newPeriodical.Id);
                child.Tick($"{++i} of {periodicals.TotalCount} Periodical(s) migrated.");
            }

        }

        return periodicalMap;
    }

    private async Task<Dictionary<int, int>> MigrateIssue(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId, Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, ChildProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.IssueRepository;
        var destinationDb = DestinationRepositoryFactory.IssueRepository;
        var issueMap = new Dictionary<int, int>();

        var issues = await sourceDb.GetIssues(libraryId, periodicalId, 1, int.MaxValue, new IssueFilter(), IssueSortByType.VolumeNumberAndIssueNumber, SortDirection.Ascending, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkRed,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using (var child = pbar.Spawn((int)issues.TotalCount, "Migrating Issues for periodical " + newPeriodicalId, options))
        {
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
                        newPeriodicalId,
                        content.VolumeNumber,
                        content.IssueNumber,
                        newFile.Id,
                        content.Language,
                        content.MimeType,
                        cancellationToken);
                }

                //TODO:  migrate pages and move to files from content
                await MigrateIssueArticle(libraryId, newLibraryId, periodicalId, newPeriodicalId, newIssue.Id, newIssue.VolumeNumber, newIssue.IssueNumber, authorMap, accountsMap, child, cancellationToken);

                issueMap.Add(issue.Id, newIssue.Id);
                child.Tick($"{++i} of {issues.TotalCount} Issues(s) migrated for periodical {periodicalId}.");
            }
        }

        return issueMap;
    }

    private async Task<Dictionary<int, int>> MigrateIssueArticle(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId, int newIssueId, int volumeNumber, int issueNumber, Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, ChildProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.IssueArticleRepository;
        var destinationDb = DestinationRepositoryFactory.IssueArticleRepository;
        var articleMap = new Dictionary<int, int>();
        var fileStore = new FileSystemStorage("../FileStore");

        var articles = await sourceDb.GetArticlesByIssue(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using (var child = pbar.Spawn(articles.Count(), $"Migrating Articles for periodical {newPeriodicalId} volume {volumeNumber} issue {issueNumber}", options))
        {
            int i = 0;
            foreach (var article in articles)
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

                    var fileName = FilePathHelper.GetIssueArticleContentFileName(content.Language);
                    var filePath = await fileStore.StoreTextFile(FilePathHelper.GetIssueArticleContentPath(newPeriodicalId, volumeNumber, issueNumber, newArticle.Id, fileName), content.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.Text = string.Empty;
                    content.FileId = file.Id;

                    await destinationDb.AddArticleContent(
                        newLibraryId,
                        new IssueArticleContentModel
                        {
                            PeriodicalId = content.PeriodicalId,
                            VolumeNumber = content.VolumeNumber,
                            IssueNumber = content.IssueNumber,
                            SequenceNumber = content.SequenceNumber,
                            Language = content.Language,
                            FileId = content.FileId,
                        },
                    cancellationToken);
                }

                child.Tick($"{++i} of {articles.Count()} Issues(s) migrated for periodical {newPeriodicalId} volume {volumeNumber} issue {issueNumber}.");
            }
        }

        return articleMap;
    }

    private async Task<long> MigrateBookShelves(int libraryId, int newLibraryId, Dictionary<int, int> accountsMap, Dictionary<int, int> booksMap, ProgressBar pbar, CancellationToken cancellationToken)
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

        using (var child = pbar.Spawn((int)bookshelves.TotalCount, "Migrating Bookshelved", options))
        {
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

                child.Tick($"{++i} of {bookshelves.TotalCount} Bookhelve(s) migrated.");

            }
        }

        return bookshelves.TotalCount;
    }

    private async Task<Dictionary<long, long>> MigrateArticles(int libraryId, int newLibraryId, Dictionary<int, int> accountsMap, Dictionary<int, int> authorMap, Dictionary<int, int> categoriesMap, ProgressBar pbar, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ArticleRepository;
        var destinationDb = DestinationRepositoryFactory.ArticleRepository;
        var fileStore = new FileSystemStorage("../FileStore");

        Dictionary<long, long> articlesMap = new Dictionary<long, long>();
        var articles = await sourceDb.GetAllArticles(libraryId, cancellationToken);
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─'
        };

        using (var child = pbar.Spawn((int)articles.Count(), "Migrating Articles", options))
        {
            int i = 0;

            foreach (var article in articles)
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
                articlesMap.Add(article.Id, newArticle.Id);

                foreach (var content in article.Contents)
                {
                    content.ArticleId = newArticle.Id;

                    var fileName = FilePathHelper.GetArticleContentFileName(content.Language);
                    var filePath = await fileStore.StoreTextFile(FilePathHelper.GetArticleContentPath(newArticle.Id, fileName), content.Text, cancellationToken);
                    var file = await AddFile(fileName, filePath, MimeTypes.Markdown, cancellationToken);
                    content.Text = string.Empty;
                    content.FileId = file.Id;
                    await destinationDb.AddArticleContent(newLibraryId, content, cancellationToken);
                }

                child.Tick($"{++i} of {articles.Count()} article(s) migrated.");

            }

        }

        return articlesMap;
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

    public async Task<FileModel> CopyFile(long fileId, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.FileRepository;
        var destinationDb = DestinationRepositoryFactory.FileRepository;

        var file = await sourceDb.GetFileById(fileId, cancellationToken);
        return await destinationDb.AddFile(file, cancellationToken);
    }

}
