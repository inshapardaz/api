using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Storage.Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;

namespace Inshapardaz.Api.Tests
{
    public class TestBase : WebApplicationFactory<Startup>
    {
        private ServiceProvider Container;
        private LibraryDataBuilder _builder;
        protected readonly bool _periodicalsEnabled;
        protected readonly AuthenticationLevel _authenticationLevel;

        public TestBase(bool periodicalsEnabled = false, AuthenticationLevel authenticationLevel = AuthenticationLevel.Unauthorized)
        {
            _periodicalsEnabled = periodicalsEnabled;
            _authenticationLevel = authenticationLevel;
        }

        public int LibraryId => _builder.Library.Id;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                if (services.Any(x => x.ServiceType == typeof(IFileStorage)))
                {
                    services.Remove(new ServiceDescriptor(typeof(IFileStorage), typeof(FileStorage), ServiceLifetime.Transient));
                    services.AddSingleton<IFileStorage, FakeFileStorage>();
                }

                services.AddTransient<LibraryDataBuilder>()
                    .AddTransient<CategoriesDataBuilder>()
                    .AddTransient<SeriesDataBuilder>()
                    .AddTransient<AuthorsDataBuilder>()
                    .AddTransient<BooksDataBuilder>()
                    .AddTransient<ChapterDataBuilder>();
                Container = services.BuildServiceProvider();

                _builder = Container.GetService<LibraryDataBuilder>();
                _builder.WithPeriodicalsEnabled(_periodicalsEnabled).Build();

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
        }
    }
}
