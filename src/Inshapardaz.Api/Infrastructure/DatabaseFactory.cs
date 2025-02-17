using MySqlRepositories = Inshapardaz.Adapters.Database.MySql.Repositories;
using SqlServerRepoisotires = Inshapardaz.Adapters.Database.SqlServer.Repositories;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Api.Infrastructure;

public static class DatabaseFactory
{
    public static IFileRepository GetFileRepository(IServiceProvider provider) =>
        GetRepository<IFileRepository, SqlServerRepoisotires.FileRepository, MySqlRepositories.FileRepository>(provider);


    public static IAccountRepository GetAccountRepository(IServiceProvider provider) =>
                GetRepository<IAccountRepository, SqlServerRepoisotires.AccountRepository, MySqlRepositories.AccountRepository>(provider);


    public static IArticleRepository GetArticleRepository(IServiceProvider provider) =>
                GetRepository<IArticleRepository, SqlServerRepoisotires.Library.ArticleRepository, MySqlRepositories.Library.ArticleRepository>(provider);

    public static IAuthorRepository GetAuthorRepository(IServiceProvider provider) =>
                GetRepository<IAuthorRepository, SqlServerRepoisotires.Library.AuthorRepository, MySqlRepositories.Library.AuthorRepository>(provider);

    public static IBookPageRepository GetBookPageRepository(IServiceProvider provider) =>
                GetRepository<IBookPageRepository, SqlServerRepoisotires.Library.BookPageRepository, MySqlRepositories.Library.BookPageRepository>(provider);

    public static IBookRepository GetBookRepository(IServiceProvider provider) =>
                GetRepository<IBookRepository, SqlServerRepoisotires.Library.BookRepository, MySqlRepositories.Library.BookRepository>(provider);

    public static IBookShelfRepository GetBookShelfRepository(IServiceProvider provider) =>
                GetRepository<IBookShelfRepository, SqlServerRepoisotires.Library.BookShelfRepository, MySqlRepositories.Library.BookShelfRepository>(provider);

    public static ICategoryRepository GetCategoryRepository(IServiceProvider provider) =>
                GetRepository<ICategoryRepository, SqlServerRepoisotires.Library.CategoryRepository, MySqlRepositories.Library.CategoryRepository>(provider);

    public static IChapterRepository GetChapterRepository(IServiceProvider provider) =>
                GetRepository<IChapterRepository, SqlServerRepoisotires.Library.ChapterRepository, MySqlRepositories.Library.ChapterRepository>(provider);

    public static ICorrectionRepository GetCorrectionRepository(IServiceProvider provider) =>
                GetRepository<ICorrectionRepository, SqlServerRepoisotires.CorrectionRepository, MySqlRepositories.CorrectionRepository>(provider);

    public static ICommonWordsRepository GetCommonWordsRepository(IServiceProvider provider) =>
                GetRepository<ICommonWordsRepository, SqlServerRepoisotires.CommonWordsRepository, MySqlRepositories.CommonWordsRepository>(provider);

    public static IIssueArticleRepository GetIssueArticleRepository(IServiceProvider provider) =>
                GetRepository<IIssueArticleRepository, SqlServerRepoisotires.Library.IssueArticleRepository, MySqlRepositories.Library.IssueArticleRepository>(provider);

    public static IIssuePageRepository GetIssuePageRepository(IServiceProvider provider) =>
                GetRepository<IIssuePageRepository, SqlServerRepoisotires.Library.IssuePageRepository, MySqlRepositories.Library.IssuePageRepository>(provider);

    public static IIssueRepository GetIssueRepository(IServiceProvider provider) =>
                GetRepository<IIssueRepository, SqlServerRepoisotires.Library.IssueRepository, MySqlRepositories.Library.IssueRepository>(provider);

    public static ILibraryRepository GetLibraryRepository(IServiceProvider provider)
    {
        var settings = GetSettings(provider);
        if (settings is not null)
        {
            if (settings.Database.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.MySql)
            {
                return provider.GetService<MySqlRepositories.Library.LibraryRepository>();
            }
            else if (settings.Database.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.SqlServer)
            {
                return provider.GetService<SqlServerRepoisotires.Library.LibraryRepository>();
            }
        }

        throw new ApplicationException($"Unsupported Database Configuration for interface {typeof(ILibraryRepository)}");
    }

    public static IPeriodicalRepository GetPeriodicalRepository(IServiceProvider provider) =>
                GetRepository<IPeriodicalRepository, SqlServerRepoisotires.Library.PeriodicalRepository, MySqlRepositories.Library.PeriodicalRepository>(provider);

    public static ISeriesRepository GetSeriesRepository(IServiceProvider provider) =>
                GetRepository<ISeriesRepository, SqlServerRepoisotires.Library.SeriesRepository, MySqlRepositories.Library.SeriesRepository>(provider);

    private static Settings GetSettings(IServiceProvider provider) => provider.GetRequiredService<IOptions<Settings>>().Value;
    private static LibraryConfiguration GetLibraryConfiguration(IServiceProvider provider) => provider.GetRequiredService<LibraryConfiguration>();

    private static TInterface GetRepository<TInterface, TSqlServer, TMySql>(IServiceProvider provider)
      where TSqlServer : TInterface
      where TMySql : TInterface
    {
        var libraryConfiguration = GetLibraryConfiguration(provider);
        if (libraryConfiguration is not null && !string.IsNullOrWhiteSpace(libraryConfiguration.ConnectionString))
        {
            if (libraryConfiguration.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.MySql)
            {
                return provider.GetService<TMySql>();
            }
            else if (libraryConfiguration.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.SqlServer)
            {
                return provider.GetService<TSqlServer>();
            }
        }
        else
        {
            var settings = GetSettings(provider);
            if (settings.Database.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.MySql)
            {
                return provider.GetService<TMySql>();
            }
            else if (settings.Database.DatabaseConnectionType == Domain.Models.Library.DatabaseTypes.SqlServer)
            {
                return provider.GetService<TSqlServer>();
            }
        }

        throw new ApplicationException($"Unsupported Database Configuration for interface {typeof(TInterface)}");
    }


}
