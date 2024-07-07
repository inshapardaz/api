using DocumentFormat.OpenXml.Math;
using Inshapardaz.Adapters.Database.MySql;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Microsoft.Extensions.Options;

namespace Inshapardaz.LibraryMigrator;

public class RepositoryFactory
{
    private readonly DatabaseTypes _dbType;

    private readonly IProvideConnection _connectionProvider;

    private ILibraryRepository _libraryRepository;
    private IAccountRepository _accountRepository;
    private IAuthorRepository _authorRepository;
    private ISeriesRepository _seriesRepository;
    private IFileRepository _fileRepository;
    private IBookShelfRepository _bookShelfRepository;
    private IIssueArticleRepository _issueArticleRepository;
    private IIssueRepository _issueRepository;
    private IPeriodicalRepository _periodicalRepository;
    private IBookPageRepository _bookPageRepository;
    private IChapterRepository _chapterRepository;
    private IBookRepository _bookRepository;
    private ICategoryRepository _categoryRepository;
    private ICorrectionRepository _correctionRepository;
    private IArticleRepository _articleRepository;
    
    public RepositoryFactory(string connectionString, DatabaseTypes dbType)
    {
        _dbType = dbType;

        var sourceSettings = new Domain.Adapters.Configuration.Settings
        {
            Database = new Domain.Adapters.Configuration.Database
            {
                DatabaseConnectionType = dbType,
                ConnectionString = connectionString
            }
        };
        var sourceSettingOption = Options.Create(sourceSettings);

        _connectionProvider = dbType switch
        {
            DatabaseTypes.MySql => new MySqlConnectionProvider(sourceSettingOption),
            DatabaseTypes.SqlServer => new SqlServerConnectionProvider(sourceSettingOption),
            _ => null
        };
    }

    public ILibraryRepository LibraryRepository
    {
        get
        {
            if (_libraryRepository == null)
            {
                _libraryRepository = _dbType switch
                {
                    DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.LibraryRepository(
                        _connectionProvider as MySqlConnectionProvider),
                    DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.LibraryRepository(
                        _connectionProvider as SqlServerConnectionProvider),
                    _ => _libraryRepository
                };
            }
            
            return _libraryRepository;
        }
    }
    public IAccountRepository AccountRepository
    {
        get
        {
            if (_accountRepository == null)
            {
                _accountRepository = _dbType switch
                {
                    DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.AccountRepository(
                        _connectionProvider as MySqlConnectionProvider),
                    DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.AccountRepository(
                        _connectionProvider as SqlServerConnectionProvider),
                    _ => _accountRepository
                };
            }

            return _accountRepository;
        }
    }
    public IAuthorRepository AuthorRepository
    {
        get
        {
            if (_authorRepository == null)
            {
                _authorRepository = _dbType switch
                {
                    DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.AuthorRepository(
                        _connectionProvider as MySqlConnectionProvider),
                    DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.AuthorRepository(
                        _connectionProvider as SqlServerConnectionProvider),
                    _ => _authorRepository
                };
            }

            return _authorRepository;
        }
    }
    public ISeriesRepository SeriesRepository
    {
        get
        {
            _seriesRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.SeriesRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.SeriesRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _seriesRepository
            };

            return _seriesRepository;
        }
    }
    public IFileRepository FileRepository
    {
        get
        {
            _fileRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.FileRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.FileRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _fileRepository
            };
            return _fileRepository;
        }
    }
    public IBookShelfRepository BookShelfRepository
    {
        get
        {
            _bookShelfRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.BookShelfRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.BookShelfRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _bookShelfRepository
            };
            return _bookShelfRepository;
        }
    }
    public IIssueArticleRepository IssueArticleRepository
    {
        get
        {
            _issueArticleRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.IssueArticleRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.IssueArticleRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _issueArticleRepository
            };

            return _issueArticleRepository;
        }
    }
    public IIssueRepository IssueRepository
    {
        get
        {
            _issueRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.IssueRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.IssueRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _issueRepository
            };
            return _issueRepository;
        }
    }
    public IPeriodicalRepository PeriodicalRepository
    {
        get
        {
            _periodicalRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.PeriodicalRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.PeriodicalRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _periodicalRepository
            };
            return _periodicalRepository;
        }
    }
    public IBookPageRepository BookPageRepository
    {
        get
        {
            _bookPageRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.BookPageRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.BookPageRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _bookPageRepository
            };
            return _bookPageRepository;
        }
    }
    public IChapterRepository ChapterRepository
    {
        get
        {
            _chapterRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.ChapterRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.ChapterRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _chapterRepository
            };
            return _chapterRepository;
        }
    }
    public IBookRepository BookRepository
    {
        get
        {
            _bookRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.BookRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.BookRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _bookRepository
            };
            return _bookRepository;
        }
    }
    public ICategoryRepository CategoryRepository
    {
        get
        {
            _categoryRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.CategoryRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.CategoryRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _categoryRepository
            };
            return _categoryRepository;
        }
    }
    public ICorrectionRepository CorrectionRepository
    {
        get
        {
            _correctionRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.CorrectionRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.CorrectionRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _correctionRepository
            };
            return _correctionRepository;
        }
    }
    public IArticleRepository ArticleRepository
    {
        get
        {
            _articleRepository = _dbType switch
            {
                DatabaseTypes.MySql => new Adapters.Database.MySql.Repositories.Library.ArticleRepository(
                    _connectionProvider as MySqlConnectionProvider),
                DatabaseTypes.SqlServer => new Adapters.Database.SqlServer.Repositories.Library.ArticleRepository(
                    _connectionProvider as SqlServerConnectionProvider),
                _ => _articleRepository
            };
            return _articleRepository;
        }
    }
}
