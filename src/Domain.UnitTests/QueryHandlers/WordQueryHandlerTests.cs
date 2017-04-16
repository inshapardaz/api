using Inshapardaz.Domain.QueryHandlers;

namespace Domain.UnitTests.QueryHandlers
{
    public class WordQueryHandlerTests : DatabaseTestFixture
    {
        private WordQueryHandler _handler;

        public WordQueryHandlerTests()
        {
            _handler = new WordQueryHandler(_database);
        }

        // TODO : Add tests here
    }
}
