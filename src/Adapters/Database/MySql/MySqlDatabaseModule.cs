using Inshapardaz.Adapters.Database.MySql.Repositories;
using Inshapardaz.Adapters.Database.MySql.Repositories.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Adapters.Database.MySql;

public static class MySqlDatabaseModule
{
    public static IServiceCollection AddMySql(this IServiceCollection services) =>
        services.AddTransient<AccountRepository>()
            .AddTransient<CorrectionRepository>()
            .AddTransient<FileRepository>()
            .AddTransient<MySqlDatabaseFileStorage>()
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
