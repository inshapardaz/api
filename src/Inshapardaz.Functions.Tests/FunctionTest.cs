using System;
using Inshapardaz.Functions.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Tests;

namespace Inshapardaz.Functions.Tests
{
    public abstract class FunctionTest
    {
        private readonly TestHostBuilder _builder;
        private readonly Startup _startup;
        public FunctionTest()
        {
            _builder = new TestHostBuilder();
            _startup = new Startup();
            _startup.Configure(_builder);
        }

        protected IServiceProvider Container => _builder.ServiceProvider;


        protected void AuthenticateAsAdmin()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.AdminRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }

        protected void  AuthenticateAsWriter()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.WriteRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }

        protected void  AuthenticateAsReader()
        {
            var authenticator = new TestAuthenticator(TestAuthenticator.ReaderRole);
            _builder.Services.AddTransient<IFunctionAppAuthenticator>(s => authenticator);
        }
    }
}
