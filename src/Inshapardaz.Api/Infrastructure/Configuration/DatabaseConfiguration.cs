using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Adapters.Database.SqlServer.Repositories;
using Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
        services.AddScoped<IBookPageRepository, BookPageRepository>();
        services.AddScoped<IPeriodicalRepository, PeriodicalRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IIssueArticleRepository, IssueArticleRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICorrectionRepository, CorrectionRepository>();
        services.AddScoped<IIssuePageRepository, IssuePageRepository>();
        services.AddScoped<IBookShelfRepository, BookShelfRepository>();
        return services;
    }

    public static readonly LoggerFactory MyLoggerFactory
        = new LoggerFactory();
}
