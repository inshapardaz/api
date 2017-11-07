using System.Threading.Tasks;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetDictionaryByIdQueryHandler _handler;
        private readonly Dictionary _dictionary1;
        private readonly Dictionary _dictionary2;

        public GetDictionaryByIdQueryHandlerTests()
        {
            _dictionary1 = new Dictionary {Id = 1, IsPublic = true };
            DbContext.Dictionary.Add(_dictionary1);
            _dictionary2 = new Dictionary {Id = 2, IsPublic = false };
            DbContext.Dictionary.Add(_dictionary2);
            
            DbContext.SaveChanges();

            _handler = new GetDictionaryByIdQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenCalledForADictionary_ShouldReturCorrectDictionary()
        {
            var result = await _handler.ExecuteAsync(new GetDictionaryByIdQuery { DictionaryId = 2 });

            result.ShouldNotBeNull();
            result.ShouldBe(_dictionary2);
        }
        
        [Fact]
        public async Task WhenCalledForIncorrectDictionary_ShouldReturnNull()
        {
            var result = await _handler.ExecuteAsync(new GetDictionaryByIdQuery {DictionaryId = 1232});
            result.ShouldBeNull();
        }
    }
}