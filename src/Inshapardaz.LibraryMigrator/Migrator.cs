using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Storage.FileSystem;

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

        Dictionary<int, int> _authorMap;
        Dictionary<int, int> _seriesMap;
        Dictionary<int, int> _categoriesMap;
        Dictionary<int, int> _booksMap;
        Dictionary<int, int> _periodicalsMap;
        Dictionary<int, int> _accountsMap;

        var sourceLibraryDb = SourceRepositoryFactory.LibraryRepository;
        var destinationLibraryDb = DestinationRepositoryFactory.LibraryRepository;

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

        await MigrateArticles(libraryId, newLibrary.Id, _accountsMap, _authorMap, _categoriesMap, cancellationToken);
        Console.WriteLine($"-----------------------------------------------------------");

        // Migrate read and favorites
        //scope.Complete();

        Console.WriteLine($"Migration Completed.");
        //}
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

    private async Task<Dictionary<int, int>> MigrateAccounts(int libraryId, int newLibraryId, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.AccountRepository;
        var destinationDb = DestinationRepositoryFactory.AccountRepository;

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
        var sourceDb = SourceRepositoryFactory.AuthorRepository;
        var destinationDb = DestinationRepositoryFactory.AuthorRepository;

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
        var sourceDb = SourceRepositoryFactory.SeriesRepository;
        var destinationDb = DestinationRepositoryFactory.SeriesRepository;

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
        var sourceDb = SourceRepositoryFactory.CategoryRepository;
        var destinationDb = DestinationRepositoryFactory.CategoryRepository;

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
        var sourceDb = SourceRepositoryFactory.BookRepository;
        var destinationDb = DestinationRepositoryFactory.BookRepository;

        Dictionary<int, int> booksMap = new Dictionary<int, int>();

        var pageNumber = 1;
        var page = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);
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

                var chaptersMap = await MigrateChapters(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, cancellationToken);
                await MigrateBookPages(libraryId, newLibraryId, book.Id, newBook.Id, accountsMap, chaptersMap, cancellationToken);

                foreach (var content in book.Contents)
                {
                    var bookContent = await CopyFile(content.FileId, cancellationToken);
                    content.BookId = newBook.Id;
                    content.FileId = bookContent.Id;

                    await destinationDb.AddBookContent(newLibraryId, bookContent.Id, content.Language, content.MimeType, cancellationToken);
                }

                Console.WriteLine($"{++i} of {page.TotalCount} Book(s) migrated.");

            }

            page = await sourceDb.GetBooks(libraryId, pageNumber++, 100, cancellationToken);
        }
        while (page.Data.Count() > 0);

        Console.WriteLine($"{booksMap.Count} Book(s) migrated.");

        return booksMap;
    }

    private async Task<Dictionary<long, long>> MigrateChapters(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ChapterRepository;
        var destinationDb = DestinationRepositoryFactory.ChapterRepository;
        var fileStore = new FileSystemStorage("../FileStore");

        var chaptersMap = new Dictionary<long, long>();
        var chapters = (await sourceDb.GetChaptersByBook(libraryId, bookId, cancellationToken)).ToArray();
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

               var filePath = await fileStore.StoreTextFile($"books/{bookId}/chapter-{newChapter.ChapterNumber}.md", content.Text, cancellationToken);
                var file = await AddFile($"chapter-{newChapter.Id}.md", filePath, MimeTypes.Markdown, cancellationToken);
                content.Text = string.Empty;
                content.FileId = file.Id;
                await destinationDb.AddChapterContent(newLibraryId, content, cancellationToken);
            }

            Console.WriteLine($"{++i} of {chapters.Length} Chapter(s) copied.");

        }

        return chaptersMap;
    }

    private static string GetChapterFileName(int bookId, long chapterId)
    {
        return $"books/{bookId}/chapter-{chapterId}.md";
    }

    private async Task MigrateBookPages(int libraryId, int newLibraryId, int bookId, int newBookId, Dictionary<int, int> accountsMap, Dictionary<long, long> chaptersMap, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.BookPageRepository;
        var destinationDb = DestinationRepositoryFactory.BookPageRepository;

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

            if (page.ReviewerAccountId.HasValue && accountsMap.ContainsKey(page.ReviewerAccountId.Value))
            {
                page.ReviewerAccountId = accountsMap[page.ReviewerAccountId.Value];
            } 
            else
            {
                page.ReviewerAccountId = null;
            }

            if (page.WriterAccountId.HasValue && accountsMap.ContainsKey(page.WriterAccountId.Value))
            {
                page.WriterAccountId = accountsMap[page.WriterAccountId.Value];
            }
            else
            {
                page.WriterAccountId = null;
            }
            
            Console.WriteLine($"{++i} of {pages.Length} Pages(s) copies.");

            //TODO: Copy page contents to file
            await destinationDb.AddPage(newLibraryId, page, cancellationToken);
        }
    }

    private async Task<Dictionary<int, int>> MigratePeriodicals(int libraryId, int newLibraryId, Dictionary<int, int> authorMap, Dictionary<int, int> categoriesMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.PeriodicalRepository;
        var destinationDb = DestinationRepositoryFactory.PeriodicalRepository;
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
        var sourceDb = SourceRepositoryFactory.IssueRepository;
        var destinationDb = DestinationRepositoryFactory.IssueRepository;
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
            await IssueMigrateArticle(libraryId, newLibraryId, periodicalId, newPeriodicalId, newIssue.Id, newIssue.VolumeNumber, newIssue.IssueNumber, authorMap, accountsMap, cancellationToken);
            Console.WriteLine($"{++i} of {issues.TotalCount} Issues(s) migrated for periodical {periodicalId}.");

            issueMap.Add(issue.Id, newIssue.Id);
        }

        return issueMap;
    }

    private async Task<Dictionary<int, int>> IssueMigrateArticle(int libraryId, int newLibraryId, int periodicalId, int newPeriodicalId, int newIssueId, int volumeNumber, int issueNumber, Dictionary<int, int> authorMap, Dictionary<int, int> accountsMap, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.IssueArticleRepository;
        var destinationDb = DestinationRepositoryFactory.IssueArticleRepository;
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

                // TODO: Migrate the content to file
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
        var sourceDb = SourceRepositoryFactory.BookShelfRepository;
        var destinationDb = DestinationRepositoryFactory.BookShelfRepository;

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

    private async Task<Dictionary<long, long>> MigrateArticles(int libraryId, int newLibraryId, Dictionary<int, int> accountsMap, Dictionary<int, int> authorMap, Dictionary<int, int> categoriesMap, CancellationToken cancellationToken)
    {
        var sourceDb = SourceRepositoryFactory.ArticleRepository;
        var destinationDb = DestinationRepositoryFactory.ArticleRepository;

        Dictionary<long, long> articlesMap = new Dictionary<long, long>();
        var articles = await sourceDb.GetAllArticles(libraryId, cancellationToken);
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

                // TODO: Migrate contents to file
                await destinationDb.AddArticleContent(newLibraryId, content, cancellationToken);
            }

            Console.WriteLine($"{++i} of {articles.Count()} Article(s) migrated.");

        }

        Console.WriteLine($"{articlesMap.Count} Book(s) migrated.");

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
