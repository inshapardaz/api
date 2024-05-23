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
    private readonly string _connectionString;
    private readonly DatabaseTypes _dbType;

    IProvideConnection ConnectionProvider { get; init; }

    public RepositoryFactory(string connectionString, DatabaseTypes dbType)
    {
        _connectionString = connectionString;
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

        if (dbType == DatabaseTypes.MySql)
        {
            ConnectionProvider = new MySqlConnectionProvider(sourceSettingOption);
        }
        else if (dbType == DatabaseTypes.SqlServer)
        {
            ConnectionProvider = new SqlServerConnectionProvider(sourceSettingOption);
        }
    }

    public ILibraryRepository LibraryRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.LibraryRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.LibraryRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IAccountRepository AccountRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.AccountRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.AccountRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }

    public IAuthorRepository AuthorRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.AuthorRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.AuthorRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public ISeriesRepository SeriesRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.SeriesRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.SeriesRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IFileRepository FileRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.FileRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.FileRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IBookShelfRepository BookShelfRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.BookShelfRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.BookShelfRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IIssueArticleRepository IssueArticleRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.IssueArticleRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.IssueArticleRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IIssueRepository IssueRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.IssueRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.IssueRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IPeriodicalRepository PeriodicalRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.PeriodicalRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.PeriodicalRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IBookPageRepository BookPageRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.BookPageRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.BookPageRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IChapterRepository ChapterRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.ChapterRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.ChapterRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public IBookRepository BookRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.BookRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.BookRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
    public ICategoryRepository CategoryRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.CategoryRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.CategoryRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }

    public ICorrectionRepository CorrectionRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.CorrectionRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.CorrectionRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }

    public IArticleRepository ArticleRepository
    {
        get
        {
            if (_dbType == DatabaseTypes.MySql)
                return new Adapters.Database.MySql.Repositories.Library.ArticleRepository(ConnectionProvider as MySqlConnectionProvider);
            else if (_dbType == DatabaseTypes.SqlServer)
                return new Adapters.Database.SqlServer.Repositories.Library.ArticleRepository(ConnectionProvider as SqlServerConnectionProvider);
            throw new NotImplementedException();
        }
    }
}
