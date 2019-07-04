using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Inshapardaz.Functions.Tests
{
    public class TestHostBuilder : IFunctionsHostBuilder
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
            Environment.SetEnvironmentVariable("DefaultDatabase", "data source=.;initial catalog=Inshapardaz;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
            Environment.SetEnvironmentVariable("API.Root", "http://localhost:7071/api");
        }
    }
}
