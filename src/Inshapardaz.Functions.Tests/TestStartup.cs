using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Books.Chapters;
using Inshapardaz.Functions.Library.Books.Chapters.Contents;
using Inshapardaz.Functions.Library.Books.Content;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Inshapardaz.Functions.Tests
{
    internal class TestStartup : Startup
    {
        private TestSqliteConnectionProvider _connectionProvider = new TestSqliteConnectionProvider();

        // TODO : Uncomment after moved everything to one schema
        //protected override IServiceCollection AddDatabaseConnection(IServiceCollection services)
        //{
        //    return services.AddSingleton<IProvideConnection>(sp => _connectionProvider);
        //}

        protected override IServiceCollection AddCustomServices(IServiceCollection services)
        {
            if (services.Any(x => x.ServiceType == typeof(IFileStorage)))
            {
                services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(FileStorage), ServiceLifetime.Transient));
                services.AddSingleton<IFileStorage>(new FakeFileStorage());
            }

            return services.AddTransient<LibraryDataBuilder>()
                    .AddTransient<CategoriesDataBuilder>()
                    .AddTransient<SeriesDataBuilder>()
                    .AddTransient<AuthorsDataBuilder>()
                    .AddTransient<BooksDataBuilder>()
                    .AddTransient<ChapterDataBuilder>()
                    .AddTransient<GetLibrary>()
                    .AddTransient<GetCategories>()
                    .AddTransient<AddCategory>()
                    .AddTransient<GetCategoryById>()
                    .AddTransient<UpdateCategory>()
                    .AddTransient<DeleteCategory>()
                    .AddTransient<GetSeries>()
                    .AddTransient<AddSeries>()
                    .AddTransient<GetSeriesById>()
                    .AddTransient<UpdateSeriesImage>()
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
                    .AddTransient<GetFavoriteBooks>()
                    .AddTransient<GetRecentReadBooks>()
                    .AddTransient<GetLatestBooks>()
                    .AddTransient<AddBook>()
                    .AddTransient<UpdateBook>()
                    .AddTransient<DeleteBook>()
                    .AddTransient<UpdateBookImage>()
                    .AddTransient<AddBookContent>()
                    .AddTransient<UpdateBookContent>()
                    .AddTransient<DeleteBookContent>()
                    .AddTransient<GetBookContent>()
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
