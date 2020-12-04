using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Inshapardaz.Database.SqlServer.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;
using System.Net.Http;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.TestHost;
using System;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests
{
    public class TestBase
    {
        protected readonly bool _periodicalsEnabled;
        protected readonly Role? _role;
        private WebApplicationFactory<Startup> _factory;
        private AccountDto _account;

        public TestBase(Role? role = null, bool periodicalsEnabled = false, bool createLibrary = true)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _role = role;

            _factory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services => ConfigureServices(services)));

            AccountBuilder = _factory.Services.GetService<AccountDataBuilder>();

            if (role.HasValue)
            {
                AccountBuilder = AccountBuilder.As(_role.Value);
                _account = AccountBuilder.Build();
            }

            if (createLibrary)
            {
                var builder = LibraryBuilder.WithPeriodicalsEnabled(_periodicalsEnabled);
                if (_account != null) builder.AssignToUser(AccountId);
                Library = builder.Build();
            }

            Client = _factory.CreateClient();

            if (_account != null)
            {
                var settings = Services.GetService<Settings>();
                var token = TokenBuilder.GenerateToken(settings, _account.Id);
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private void ConfigureServices(IServiceCollection services)
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
                .AddTransient<ChapterDataBuilder>()
                .AddTransient<AccountDataBuilder>();

            /*services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", op => { });*/

            //services.AddScoped<TestClaimsProvider>(svc => svc.GetService<TestClaimsProvider>().WithAccount(_account?.Id));

            /*if (services.Any(x => x.ServiceType == typeof(IUserHelper)))
             {
                 services.Remove(new ServiceDescriptor(typeof(IUserHelper), typeof(UserHelper), ServiceLifetime.Transient));
                 services.AddScoped<IUserHelper, TestUserHelper>();
             }
             */
        }

        public HttpClient Client { get; }

        public int AccountId => _account?.Id ?? 0;

        protected int LibraryId => _libraryBuilder.Library.Id;

        protected IServiceProvider Services => _factory.Services;

        protected LibraryDto Library { get; }

        protected IDbConnection DatabaseConnection => _factory.Services.GetService<IProvideConnection>().GetConnection();

        protected FakeFileStorage FileStore => _factory.Services.GetService<IFileStorage>() as FakeFileStorage;

        protected AccountDataBuilder _accountBuilder;
        protected LibraryDataBuilder _libraryBuilder;
        protected AuthorsDataBuilder _authorBuilder;
        protected SeriesDataBuilder _seriesDataBuilder;
        protected CategoriesDataBuilder _categoriesDataBuilder;
        protected BooksDataBuilder _booksDataBuilder;
        protected ChapterDataBuilder _chapterDataBuilder;
        protected Role? CurrentAuthenticationLevel => _role;

        protected AccountDataBuilder AccountBuilder { get; }

        protected LibraryDataBuilder LibraryBuilder
        {
            get
            {
                if (_libraryBuilder == null)
                {
                    _libraryBuilder = _factory.Services.GetService<LibraryDataBuilder>();
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
                    _authorBuilder = _factory.Services.GetService<AuthorsDataBuilder>();
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
                    _seriesDataBuilder = _factory.Services.GetService<SeriesDataBuilder>();
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
                    _categoriesDataBuilder = _factory.Services.GetService<CategoriesDataBuilder>();
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
                    _booksDataBuilder = _factory.Services.GetService<BooksDataBuilder>();
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
                    _chapterDataBuilder = _factory.Services.GetService<ChapterDataBuilder>();
                }

                return _chapterDataBuilder;
            }
        }

        protected virtual void Cleanup()
        {
            _accountBuilder?.CleanUp();
            _libraryBuilder?.CleanUp();
            _authorBuilder?.CleanUp();
            _seriesDataBuilder?.CleanUp();
            _categoriesDataBuilder?.CleanUp();
            _booksDataBuilder?.CleanUp();
            _chapterDataBuilder?.CleanUp();
        }
    }
}
