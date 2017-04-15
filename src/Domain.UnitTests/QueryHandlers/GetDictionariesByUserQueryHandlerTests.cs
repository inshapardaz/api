using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.UnitTests.Fakes;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Xunit;

namespace Domain.UnitTests.QueryHandlers
{
    public class GetDictionariesByUserQueryHandlerTests
    {
        private GetDictionariesByUserQueryHandler hander;

        public GetDictionariesByUserQueryHandlerTests()
        {
            hander = new GetDictionariesByUserQueryHandler(new MockDatabase());
        }

        [Fact]
        public void WhenCallingForAnonymous_ShouldReturnAlPublicDictionaries()
        {
            
        }
    }
}
