using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests
{
    public class TestBase : WebApplicationFactory<Startup>
    {
        private LibraryDataBuilder _builder;
        protected readonly bool _periodicalsEnabled;
        protected readonly Permission _authenticationLevel;

        public TestBase(Permission Permission = Permission.Unauthorised, bool periodicalsEnabled = false)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _authenticationLevel = Permission;
            Client = CreateClient();
            _builder = Services.GetService<LibraryDataBuilder>();
            _builder.WithPeriodicalsEnabled(_periodicalsEnabled).Build();
        }

        public HttpClient Client { get; private set; }

        protected int LibraryId => _builder.Library.Id;
        protected LibraryDto Library => _builder.Library;

        protected IDbConnection DatabaseConnection => Services.GetService<IProvideConnection>().GetConnection();

        protected FakeFileStorage FileStore => Services.GetService<IFileStorage>() as FakeFileStorage;

        protected AuthorsDataBuilder _authorBuilder;
        protected SeriesDataBuilder _seriesDataBuilder;
        protected Permission CurrentAuthenticationLevel => _authenticationLevel;

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

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (services.Any(x => x.ServiceType == typeof(IFileStorage)))
                {
                    services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(FileStorage), ServiceLifetime.Transient));
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

                services.AddScoped<TestClaimsProvider>(_ => TestClaimsProvider.WithAuthLevel(_authenticationLevel));

                if (services.Any(x => x.ServiceType == typeof(IUserHelper)))
                {
                    services.Remove(new ServiceDescriptor(typeof(IUserHelper), typeof(UserHelper), ServiceLifetime.Transient));
                    services.AddScoped<IUserHelper, TestUserHelper>();
                }
            });
        }

        protected virtual void Cleanup()
        {
            _builder.CleanUp();
            _authorBuilder?.CleanUp();
            _seriesDataBuilder?.CleanUp();
        }
    }
}
