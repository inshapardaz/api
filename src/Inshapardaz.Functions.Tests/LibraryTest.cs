using Inshapardaz.Functions.Tests.DataBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Tests
{
    public class LibraryTest : FunctionTest
    {
        private LibraryDataBuilder _builder;

        public int LibraryId => _builder.Library.Id;

        public LibraryTest()
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            _builder.Build();
        }

        protected override void Cleanup()
        {
            _builder.CleanUp();
        }
    }
}
