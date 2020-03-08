using System;
using System.Data;
using FluentMigrator.Runner;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Chapters.Contents;
using Inshapardaz.Functions.Library.Books.Files;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Ports.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public abstract class FunctionTest
    {
        private readonly TestHostBuilder _builder;
        private readonly Startup _startup;
        private SqliteConnectionProvider _connectionProvider;

        protected FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new Startup();

            InitializeDatabaseMigration(_builder.Services);
            _connectionProvider = new SqliteConnectionProvider();

            _builder.Services.AddSingleton<IProvideConnection>(sp => _connectionProvider);
            _builder.Services.AddTransient<LibraryDataBuilder>()
                             .AddTransient<CategoriesDataBuilder>()
                             .AddTransient<SeriesDataBuilder>()
                             .AddTransient<AuthorsDataBuilder>()
                             .AddTransient<BooksDataBuilder>()
                             .AddTransient<ChapterDataBuilder>()
                             .AddSingleton<IFileStorage>(new FakeFileStorage());

            RegisterHandlers(_builder);
            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;

        protected IDbConnection DatabaseConnection => Container.GetService<IProvideConnection>().GetConnection();

        protected void InitializeDatabaseMigration(IServiceCollection services)
        {
            var serviceProvider = services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString("DataSource=:memory:")
                    .ScanIn(typeof(Database.Migrations.Migration000001_CreateLibrarySchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        protected virtual void Cleanup()
        {
        }

        private void RegisterHandlers(TestHostBuilder builder)
        {
            builder.Services.AddTransient<GetLibrary>()
                   .AddTransient<GetCategories>()
                   .AddTransient<AddCategory>()
                   .AddTransient<GetCategoryById>()
                   .AddTransient<UpdateCategory>()
                   .AddTransient<DeleteCategory>()
                   .AddTransient<GetSeries>()
                   .AddTransient<AddSeries>()
                   .AddTransient<GetSeriesById>()
                   .AddTransient<UpdateSeries>()
                   .AddTransient<DeleteSeries>()
                   .AddTransient<GetAuthors>()
                   .AddTransient<GetAuthorById>()
                   .AddTransient<AddAuthor>()
                   .AddTransient<UpdateAuthor>()
                   .AddTransient<DeleteAuthor>()
                   .AddTransient<UpdateAuthorImage>()
                   .AddTransient<GetBooks>()
                   .AddTransient<GetBookById>()
                   .AddTransient<GetBooksByAuthor>()
                   .AddTransient<GetBooksByCategory>()
                   .AddTransient<GetBooksBySeries>()
                   .AddTransient<GetFavoriteBooks>()
                   .AddTransient<GetRecentReadBooks>()
                   .AddTransient<GetLatestBooks>()
                   .AddTransient<AddBook>()
                   .AddTransient<UpdateBook>()
                   .AddTransient<DeleteBook>()
                   .AddTransient<UpdateBookImage>()
                   .AddTransient<AddBookFile>()
                   .AddTransient<UpdateBookFile>()
                   .AddTransient<DeleteBookFile>()
                   .AddTransient<GetBookFiles>()
                   .AddTransient<GetChapterById>()
                   .AddTransient<AddChapter>()
                   .AddTransient<UpdateChapter>()
                   .AddTransient<DeleteChapter>()
                   .AddTransient<GetChaptersByBook>()
                   .AddTransient<GetChapterContents>()
                   .AddTransient<AddChapterContents>()
                   .AddTransient<UpdateChapterContents>()
                   .AddTransient<DeleteChapterContents>();
        }
    }
}
