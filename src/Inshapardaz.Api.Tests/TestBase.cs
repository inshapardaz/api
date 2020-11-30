using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Database.SqlServer.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests
{
    public class TestBase : WebApplicationFactory<Startup>
    {
        protected readonly bool _periodicalsEnabled;
        protected readonly Permission _authenticationLevel;

        public TestBase(Permission Permission = Permission.Unauthorised, bool periodicalsEnabled = false, bool createLibrary = true)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _authenticationLevel = Permission;
            Client = CreateClient();
            if (createLibrary)
            {
                LibraryBuilder.WithPeriodicalsEnabled(_periodicalsEnabled).Build();
            }
        }

        public HttpClient Client { get; private set; }
        public readonly Guid UserId = Guid.NewGuid();

        protected int LibraryId => _libraryBuilder.Library.Id;
        protected LibraryDto Library => _libraryBuilder.Library;

        protected IDbConnection DatabaseConnection => Services.GetService<IProvideConnection>().GetConnection();

        protected FakeFileStorage FileStore => Services.GetService<IFileStorage>() as FakeFileStorage;

        protected LibraryDataBuilder _libraryBuilder;
        protected AuthorsDataBuilder _authorBuilder;
        protected SeriesDataBuilder _seriesDataBuilder;
        protected CategoriesDataBuilder _categoriesDataBuilder;
        protected BooksDataBuilder _booksDataBuilder;
        protected ChapterDataBuilder _chapterDataBuilder;
        protected Permission CurrentAuthenticationLevel => _authenticationLevel;

        protected LibraryDataBuilder LibraryBuilder
        {
            get
            {
                if (_libraryBuilder == null)
                {
                    _libraryBuilder = Services.GetService<LibraryDataBuilder>();
                }

                return _libraryBuilder;
            }
        }

        protected AuthorsDataBuilder AuthorBuilder
        {
            get
            {
                if (_authorBuilder == null)
                {
                    _authorBuilder = Services.GetService<AuthorsDataBuilder>();
                }

                return _authorBuilder;
            }
        }

        protected SeriesDataBuilder SeriesBuilder
        {
            get
            {
                if (_seriesDataBuilder == null)
                {
                    _seriesDataBuilder = Services.GetService<SeriesDataBuilder>();
                }

                return _seriesDataBuilder;
            }
        }

        protected CategoriesDataBuilder CategoryBuilder
        {
            get
            {
                if (_categoriesDataBuilder == null)
                {
                    _categoriesDataBuilder = Services.GetService<CategoriesDataBuilder>();
                }

                return _categoriesDataBuilder;
            }
        }

        protected BooksDataBuilder BookBuilder
        {
            get
            {
                if (_booksDataBuilder == null)
                {
                    _booksDataBuilder = Services.GetService<BooksDataBuilder>();
                }

                return _booksDataBuilder;
            }
        }

        protected ChapterDataBuilder ChapterBuilder
        {
            get
            {
                if (_chapterDataBuilder == null)
                {
                    _chapterDataBuilder = Services.GetService<ChapterDataBuilder>();
                }

                return _chapterDataBuilder;
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (services.Any(x => x.ServiceType == typeof(IFileStorage)))
                {
                    services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(DatabaseFileStorage), ServiceLifetime.Transient));
                    services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(AzureFileStorage), ServiceLifetime.Transient));
                    services.AddSingleton<IFileStorage>(new FakeFileStorage());
                }

                services.AddTransient<LibraryDataBuilder>()
                    .AddTransient<CategoriesDataBuilder>()
                    .AddTransient<SeriesDataBuilder>()
                    .AddTransient<AuthorsDataBuilder>()
                    .AddTransient<BooksDataBuilder>()
                    .AddTransient<ChapterDataBuilder>();

                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", op => { });

                services.AddScoped<TestClaimsProvider>(_ => TestClaimsProvider.WithAuthLevel(_authenticationLevel, UserId));

                if (services.Any(x => x.ServiceType == typeof(IUserHelper)))
                {
                    services.Remove(new ServiceDescriptor(typeof(IUserHelper), typeof(UserHelper), ServiceLifetime.Transient));
                    services.AddScoped<IUserHelper, TestUserHelper>();
                }
            });
        }

        protected virtual void Cleanup()
        {
            _libraryBuilder.CleanUp();
            _authorBuilder?.CleanUp();
            _seriesDataBuilder?.CleanUp();
            _categoriesDataBuilder?.CleanUp();
            _booksDataBuilder?.CleanUp();
            _chapterDataBuilder?.CleanUp();
        }
    }
}
