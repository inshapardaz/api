using System;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Chapters.Contents;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Ports.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public abstract class FunctionTest
    {
        private readonly TestHostBuilder _builder;
        private readonly Startup _startup;
        private SqliteConnection _connection;

        protected FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new Startup();
            
            DatabaseContext = CreateDbContext();
            
            _builder.Services.AddSingleton<IDatabaseContext>(sp => DatabaseContext);
            _builder.Services.AddTransient<CategoriesDataBuilder>()
                             .AddTransient<SeriesDataBuilder>()
                             .AddTransient<AuthorsDataBuilder>()
                             .AddTransient<BooksDataBuilder>()
                             .AddTransient<ChapterDataBuilder>()
                             .AddSingleton<IFileStorage>(new FakeFileStorage());

            RegisterHandlers(_builder);
            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;

        protected IDatabaseContext DatabaseContext { get; private set; }

        
        private IDatabaseContext CreateDbContext()
        {
            if (_connection != null)
            {
                throw new Exception("connection already created");
            }
            
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                          .UseSqlite(_connection)
                          .EnableSensitiveDataLogging()
                          .EnableDetailedErrors()
                          .Options;

             var context = new DatabaseContext(options);
             context.Database.EnsureCreated();
             return context;
        }

        protected void Cleanup()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private void RegisterHandlers(TestHostBuilder builder)
        {
            builder.Services.AddTransient<GetEntry>()
                   .AddTransient<GetLanguages>()
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
                   .AddTransient<GetBooks>()
                   .AddTransient<GetBookById>()
                   .AddTransient<AddBook>()
                   .AddTransient<UpdateBook>()
                   .AddTransient<DeleteBook>()
                   .AddTransient<GetChapterById>()
                   .AddTransient<AddChapter>()
                   .AddTransient<UpdateChapter>()
                   .AddTransient<DeleteChapter>()
                   .AddTransient<GetChaptersByBook>()
                   .AddTransient<GetChapterContents>();
        }
    }
}
