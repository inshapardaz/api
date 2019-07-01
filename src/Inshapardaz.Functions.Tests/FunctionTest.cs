using System;

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
    }
}
