using Inshapardaz.Adapters.Database.SqlServer.Repositories;
using Inshapardaz.Adapters.Database.SqlServer.Repositories.Library;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Adapters.Database.SqlServer;

public static class SqlServerDatabaseModule
{
    public static IServiceCollection AddSqlServer(this IServiceCollection services) =>
        services.AddTransient<AccountRepository>()
            .AddTransient<CorrectionRepository>()
            .AddTransient<FileRepository>()
            .AddTransient<SqlServerDatabaseFileStorage>()
            .AddTransient<ArticleRepository>()
            .AddTransient<AuthorRepository>()
            .AddTransient<BookPageRepository>()
            .AddTransient<BookRepository>()
            .AddTransient<BookShelfRepository>()
            .AddTransient<CategoryRepository>()
            .AddTransient<ChapterRepository>()
            .AddTransient<IssueArticleRepository>()
            .AddTransient<IssuePageRepository>()
            .AddTransient<IssueRepository>()
            .AddTransient<LibraryRepository>()
            .AddTransient<PeriodicalRepository>()
            .AddTransient<SeriesRepository>()
            .AddTransient<CommonWordsRepository>();
}
