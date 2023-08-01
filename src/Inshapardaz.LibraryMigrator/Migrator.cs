using Inshapardaz.Database.SqlServer;
using Inshapardaz.Database.SqlServer.Repositories;
using Inshapardaz.Database.SqlServer.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.LibraryMigrator;
public class Migrator
{
    SqlServerConnectionProvider SourceConnectionProvider { get; init; }
    SqlServerConnectionProvider DestinationConnectionProvider { get; init; }

    public Migrator(string source, string destination)
    {
        SourceConnectionProvider = new SqlServerConnectionProvider(source, new LibraryConfiguration());
        DestinationConnectionProvider = new SqlServerConnectionProvider(destination, new LibraryConfiguration());
    }

    public async Task Migrate(int libraryId, CancellationToken cancellationToken)
    {
        Dictionary<int, int> _authorMap;
        Dictionary<int, int> _seriesMap;
        Dictionary<int, int> _categoriesMap;
        Dictionary<int, int> _booksMap;
        Dictionary<int, int> _periodicalsMap;
        Dictionary<int, int> _accountsMap;

        var sourceLibraryDb = new LibraryRepository(SourceConnectionProvider);
        var destinationLibraryDb = new LibraryRepository(DestinationConnectionProvider);

        //TransactionManager.ImplicitDistributedTransactions = true;

        //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //{
        //TransactionInterop.GetTransmitterPropagationToken(Transaction.Current);

        var sourceLibrary = await sourceLibraryDb.GetLibraryById(libraryId, cancellationToken);

        sourceLibrary.DatabaseConnection = string.Empty;
        if (sourceLibrary.ImageId.HasValue)
        {
            var libraryImage = await CopyFile(sourceLibrary.ImageId.Value, cancellationToken);
            sourceLibrary.ImageId = libraryImage.Id;
        }

        Console.WriteLine("Migration started.");

        var newLibrary = await destinationLibraryDb.AddLibrary(sourceLibrary, cancellationToken);

        Console.WriteLine($"New library is {newLibrary.Id}");

        _accountsMap = await MigrateAccounts(libraryId, newLibrary.Id, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        _authorMap = await MigrateAuthors(libraryId, newLibrary.Id, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        _seriesMap = await MigrateSeries(libraryId, newLibrary.Id, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        _categoriesMap = await MigrateCategories(libraryId, newLibrary.Id, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        _booksMap = await MigrateBooks(libraryId, newLibrary.Id, _authorMap, _seriesMap, _categoriesMap, _accountsMap, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        _periodicalsMap = await MigratePeriodicals(libraryId, newLibrary.Id, _authorMap, _categoriesMap, _accountsMap, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        var bookshelfCount = await MigrateBookShelves(libraryId, newLibrary.Id, _accountsMap, _booksMap, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        //scope.Complete();

        Console.WriteLine($"Migration Completed.");
        //}
    }

    private async Task<Dictionary<int, int>> MigrateAccounts(int libraryId, int newLibraryId, CancellationToken cancellationToken)
    {
        var sourceDb = new AccountRepository(SourceConnectionProvider);
        var destinationDb = new AccountRepository(DestinationConnectionProvider);

        Dictionary<int, int> accountsMap = new Dictionary<int, int>();
        var accounts = await sourceDb.GetAccounts(1, int.MaxValue, cancellationToken);

        int i = 0;
        foreach (var account in accounts.Data)
        {
            var existingAccount = await destinationDb.GetAccountByEmail(account.Email, cancellationToken);

            if (existingAccount == null)
            {
                existingAccount = await destinationDb.AddAccount(account, cancellationToken);
            }
            
            await destinationDb.AddAccountToLibrary(newLibraryId, existingAccount.Id, account.Role, cancellationToken);
            
            Console.WriteLine($"{++i} of {accounts.TotalCount} Account(s) migrated.");

            accountsMap.Add(account.Id, existingAccount.Id);
        }

        Console.WriteLine($"{accountsMap.Count} Account(s) migrated.");

        return accountsMap;
    }

    private async Task<Dictionary<int, int>> MigrateAuthors(int libraryId, int newLibraryId, CancellationToken cancellationToken)
    {
        var sourceDb = new AuthorRepository(SourceConnectionProvider);
        var destinationDb = new AuthorRepository(DestinationConnectionProvider);

        Dictionary<int, int> authorMap = new Dictionary<int, int>();
        var authors = await sourceDb.GetAuthors(libraryId, null, 1, int.MaxValue, cancellationToken);
        int i = 0;

        foreach (var author in authors.Data)
        {
            if (author.ImageId.HasValue)
            {
                var authorImage = await CopyFile(author.ImageId.Value, cancellationToken);
                author.ImageId = authorImage.Id;
            }

            var newAuthor = await destinationDb.AddAuthor(newLibraryId, author, cancellationToken);
            Console.WriteLine($"{++i} of {authors.TotalCount} Author(s) migrated.");

            authorMap.Add(author.Id, newAuthor.Id);
        }

        Console.WriteLine($"{authorMap.Count} Author(s) migrated.");

        return authorMap;
    }

    private async Task<Dictionary<int, int>> MigrateSeries(int libraryId, int newLibraryId, CancellationToken cancellationToken)
    {
        var sourceDb = new SeriesRepository(SourceConnectionProvider);
        var destinationDb = new SeriesRepository(DestinationConnectionProvider);

        Dictionary<int, int> seriesMap = new Dictionary<int, int>();
        var series = await sourceDb.GetSeries(libraryId, 1, int.MaxValue, cancellationToken);
        int i = 0;

        foreach (var serie in series.Data)
        {
            if (serie.ImageId.HasValue)
            {
                var serieImage = await CopyFile(serie.ImageId.Value, cancellationToken);
                serie.ImageId = serieImage.Id;
            }

            var newSerie = await destinationDb.AddSeries(newLibraryId, serie, cancellationToken);
            Console.WriteLine($"{++i} of {series.TotalCount} Series(s) migrated.");

            seriesMap.Add(serie.Id, newSerie.Id);
        }

        Console.WriteLine($"{seriesMap.Count} Series(s) migrated.");

        return seriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateCategories(int libraryId, int newLibraryId, CancellationToken cancellationToken)
    {
        var sourceDb = new CategoryRepository(SourceConnectionProvider);
        var destinationDb = new CategoryRepository(DestinationConnectionProvider);

        Dictionary<int, int> categoriesMap = new Dictionary<int, int>();
        var categories = (await sourceDb.GetCategories(libraryId, cancellationToken)).ToArray();
        int i = 0;

        foreach (var category in categories)
        {
            var newCategory = await destinationDb.AddCategory(newLibraryId, category, cancellationToken);
            Console.WriteLine($"{++i} of {categories.Length} Categories migrated.");
            categoriesMap.Add(category.Id, newCategory.Id);
        }

        Console.WriteLine($"{categoriesMap.Count} Series(s) migrated.");

        return categoriesMap;
    }

    private async Task<Dictionary<int, int>> MigrateBooks(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> seriesMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = new BookRepository(SourceConnectionProvider);
        var destinationDb = new BookRepository(DestinationConnectionProvider);

        Dictionary<int, int> booksMap = new Dictionary<int, int>();
        var books = await sourceDb.GetBooks(libraryId, 1, int.MaxValue, cancellationToken);
        int i = 0;

        foreach (var book in books.Data)
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

            var chaptersMap = await MigrateChapters(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, cancellationToken);
            await MigrateBookPages(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, chaptersMap, cancellationToken);

            Console.WriteLine($"{++i} of {books.TotalCount} Book(s) migrated.");

        }

        Console.WriteLine($"{booksMap.Count} Book(s) migrated.");

        return booksMap;
    }

    private async Task<Dictionary<int, int>> MigrateChapters(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = new ChapterRepository(SourceConnectionProvider);
        var destinationDb = new ChapterRepository(DestinationConnectionProvider);

        Dictionary<int, int> chaptersMap = new Dictionary<int, int>();
        var chapters = (await sourceDb.GetChaptersByBook(libraryId, bookId, cancellationToken)).ToArray();
        int i = 0;

        foreach (var chapter in chapters)
        {
            chapter.BookId = newBookId;
            chapter.ReviewerAccountId = chapter.ReviewerAccountId.HasValue ? accountsMap[chapter.ReviewerAccountId.Value] : null;
            chapter.WriterAccountId = chapter.WriterAccountId.HasValue ? accountsMap[chapter.WriterAccountId.Value] : null;

            var newChapter = await destinationDb.AddChapter(newLibraryId, newBookId, chapter, cancellationToken);

            chaptersMap.Add(chapter.Id, newChapter.Id);

            var contents = await sourceDb.GetChapterContents(libraryId, bookId, chapter.ChapterNumber, cancellationToken);

            foreach (var content in contents)
            {
                content.BookId = newBookId;
                content.ChapterId = newChapter.Id;
                await destinationDb.AddChapterContent(newLibraryId, content, cancellationToken);
            }

            Console.WriteLine($"{++i} of {chapters.Length} Chapter(s) copies.");

        }

        return chaptersMap;
    }

    private async Task MigrateBookPages(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, Dictionary<int, int> chaptersMap, CancellationToken cancellationToken)
    {
        var sourceDb = new BookPageRepository(SourceConnectionProvider);
        var destinationDb = new BookPageRepository(DestinationConnectionProvider);

        var pages = (await sourceDb.GetAllPagesByBook(libraryId, bookId, cancellationToken)).ToArray();
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

            page.ReviewerAccountId = page.ReviewerAccountId.HasValue ? accountsMap[page.ReviewerAccountId.Value] : null;
            page.WriterAccountId = page.WriterAccountId.HasValue ? accountsMap[page.WriterAccountId.Value] : null;
            Console.WriteLine($"{++i} of {pages.Length} Pages(s) copies.");

            await destinationDb.AddPage(newLibraryId, page, cancellationToken);
        }
    }

    private async Task<Dictionary<int, int>> MigratePeriodicals(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = new PeriodicalRepository(SourceConnectionProvider);
        var destinationDb = new PeriodicalRepository(DestinationConnectionProvider);
        var periodicalMap = new Dictionary<int, int>();

        var periodicals = await sourceDb.GetPeriodicals(libraryId, null, 1, int.MaxValue, new PeriodicalFilter(), PeriodicalSortByType.DateCreated, SortDirection.Ascending, cancellationToken);
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

            await MigrateIssue(libraryId, newLibraryId, periodical.Id, newPeriodical.Id, authorMap, accountsMap, cancellationToken);
            Console.WriteLine($"{++i} of {periodicals.TotalCount} Periodical(s) migrated.");

            periodicalMap.Add(periodical.Id, newPeriodical.Id);
        }

        Console.WriteLine($"{periodicalMap.Count} Periodical(s) migrated.");

        return periodicalMap;
    }

    private async Task<Dictionary<int, int>> MigrateIssue(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId, Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = new IssueRepository(SourceConnectionProvider);
        var destinationDb = new IssueRepository(DestinationConnectionProvider);
        var issueMap = new Dictionary<int, int>();

        var issues = await sourceDb.GetIssues(libraryId, periodicalId, 1, int.MaxValue, new IssueFilter(), IssueSortByType.VolumeNumberAndIssueNumber, SortDirection.Ascending, cancellationToken);
        int i = 0;

        foreach (var issue in issues.Data)
        {
            if (issue.ImageId.HasValue)
            {
                var issueImage = await CopyFile(issue.ImageId.Value, cancellationToken);
                issue.ImageId = issueImage.Id;
            }

            var newIssue = await destinationDb.AddIssue(newLibraryId, periodicalId, issue, cancellationToken);

            var contents = await sourceDb.GetIssueContents(libraryId, periodicalId, issue.VolumeNumber, issue.IssueNumber, cancellationToken);

            foreach (var content in contents)
            {
                content.PeriodicalId = periodicalId;

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

            await IssueMigrateArticle(libraryId, newLibraryId, periodicalId, newPeriodicalId, newIssue.Id, newIssue.VolumeNumber, newIssue.IssueNumber, authorMap, accountsMap, cancellationToken);
            Console.WriteLine($"{++i} of {issues.TotalCount} Issues(s) migrated for periodical {periodicalId}.");

            issueMap.Add(issue.Id, newIssue.Id);
        }

        return issueMap;
    }

    private async Task<Dictionary<int, int>> IssueMigrateArticle(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId, int newIssueId, int volumeNumber, int issueNumber, Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = new IssueArticleRepository(SourceConnectionProvider);
        var destinationDb = new IssueArticleRepository(DestinationConnectionProvider);
        var articleMap = new Dictionary<int, int>();

        var articles = await sourceDb.GetArticlesByIssue(libraryId, periodicalId, volumeNumber, issueNumber, cancellationToken);
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

                await destinationDb.AddArticleContent(newLibraryId,
                    newPeriodicalId,
                    content.VolumeNumber,
                    content.IssueNumber,
                    content.SequenceNumber,
                    content.Language,
                    content.Text,
                    cancellationToken);
            }

        }
        return articleMap;
    }

    private async Task<long> MigrateBookShelves(int libraryId, int newLibraryId, Dictionary<int, int> accountsMap, Dictionary<int, int> booksMap, CancellationToken cancellationToken)
    {
        var sourceDb = new BookShelfRepository(SourceConnectionProvider);
        var destinationDb = new BookShelfRepository(DestinationConnectionProvider);

        var bookshelves = await sourceDb.GetAllBookShelves(libraryId, 1, int.MaxValue, cancellationToken);

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
        }

        return bookshelves.TotalCount;
    }

    public async Task<FileModel> CopyFile(int fileId, CancellationToken cancellationToken)
    {
        var sourceDb = new FileRepository(SourceConnectionProvider);
        var destinationDb = new FileRepository(DestinationConnectionProvider);

        var file = await sourceDb.GetFileById(fileId, cancellationToken);
        return await destinationDb.AddFile(file, cancellationToken);
    }

}
