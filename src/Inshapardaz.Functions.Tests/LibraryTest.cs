using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public class LibraryTest<T> : FunctionTest
    {
        private LibraryDataBuilder _builder;
        protected T handler;

        public int LibraryId => _builder.Library.Id;
        public bool PeriodicalsEnabled => _builder.Library.SupportsPeriodicals;

        public LibraryAsserts Check => Container.GetService<LibraryAsserts>();

        public LibraryTest()
            : this(true)
        {
        }

        public LibraryTest(bool enablePeriodicals)
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            _builder.WithPeriodicalsEnabled(enablePeriodicals).Build();

            handler = Container.GetService<T>();
        }

        protected override void Cleanup()
        {
            _builder.CleanUp();
        }
    }
}
