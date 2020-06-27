using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Inshapardaz.Functions.Tests
{
    public class TestHostBuilder : IWebJobsBuilder
    {
        private readonly ServiceCollection _services;
        private IServiceProvider _serviceProvider;

        public TestHostBuilder()
        {
            SetupEnvironment();

            _services = new ServiceCollection();
        }

        public IServiceCollection Services => _services;

        public IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = _services.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }

        private void SetupEnvironment()
        {
            Environment.SetEnvironmentVariable("Authentication.Audience", "http://api.inshapardaz.org");
            Environment.SetEnvironmentVariable("Authentication.Authority", "inshapardaz.eu.auth0.com");
            Environment.SetEnvironmentVariable("Authentication.IssuerToken", "http://api.inshapardaz.org");
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DefaultDatabase")))
                Environment.SetEnvironmentVariable("DefaultDatabase", "Server=tcp:omar-azure01.database.windows.net,1433;Initial Catalog=inshapardaz-test;Persist Security Info=False;User ID=omar;Password=Celeron30);MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            Environment.SetEnvironmentVariable("API.Root", "http://localhost:7071/api");
            Environment.SetEnvironmentVariable("FileStorageConnectionString", "UseDevelopmentStorage=true");
        }
    }
}
