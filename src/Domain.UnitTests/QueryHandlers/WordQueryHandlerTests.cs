using Inshapardaz.Domain.QueryHandlers;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class WordQueryHandlerTests : DatabaseTestFixture
    {
        private GetWordsPagesQueryHandler _handler;

        public WordQueryHandlerTests()
        {
            _handler = new GetWordsPagesQueryHandler(_database);
        }

        // TODO : Add tests here
    }
}
