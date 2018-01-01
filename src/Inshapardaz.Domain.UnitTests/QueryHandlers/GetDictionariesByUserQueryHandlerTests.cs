using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    [TestFixture]
    public class GetDictionariesByUserQueryHandlerTests : DatabaseTest
    {
        private readonly GetDictionariesByUserQueryHandler _handler;
        private readonly Guid _userId1;
        private readonly Guid _userId2;
        private readonly Dictionary _dictionary1;
        private readonly Dictionary _dictionary2;
        private readonly Dictionary _dictionary3;
        private readonly Dictionary _dictionary4;

        public GetDictionariesByUserQueryHandlerTests()
        {
            _userId1 = Guid.NewGuid();
            _userId2 = Guid.NewGuid();

            _dictionary1 = new Dictionary {Id = 1, IsPublic = true, UserId = _userId1};
            DbContext.Dictionary.Add(_dictionary1);

            _dictionary2 = new Dictionary {Id = 2, IsPublic = true, UserId = _userId2};
            DbContext.Dictionary.Add(_dictionary2);

            _dictionary3 = new Dictionary {Id = 3, IsPublic = false, UserId = _userId2};
            DbContext.Dictionary.Add(_dictionary3);

            _dictionary4 = new Dictionary {Id = 4, IsPublic = false, UserId = _userId1};
            DbContext.Dictionary.Add(_dictionary4);

            DbContext.SaveChanges();

            _handler = new GetDictionariesByUserQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenCallingForAnonymous_ShouldReturnAllPublicDictionaries()
        {
            var result = await _handler.ExecuteAsync(new GetDictionariesByUserQuery());

            var dictionaries = result.ToArray();

            dictionaries.ShouldNotBeNull();
            dictionaries.Length.ShouldBe(2);

            dictionaries[0].ShouldBe(_dictionary1);
            dictionaries[0].Id.ShouldBe(1);
            dictionaries[0].IsPublic.ShouldBeTrue();
            dictionaries[1].Id.ShouldBe(2);
            dictionaries[1].IsPublic.ShouldBeTrue();
        }

        [Test]
        public async Task WhenCalledForAUser_ShouldReturnPublicAndPrivateDictionaries()
        {
            var result = await _handler.ExecuteAsync(new GetDictionariesByUserQuery {UserId = _userId2 });

            var dictionaries = result.ToArray();

            dictionaries.ShouldNotBeNull();
            dictionaries.Length.ShouldBe(3);

            dictionaries[0].Id.ShouldBe(1);
            dictionaries[0].IsPublic.ShouldBeTrue();
            dictionaries[1].Id.ShouldBe(2);
            dictionaries[1].IsPublic.ShouldBeTrue();
            dictionaries[2].Id.ShouldBe(3);
            dictionaries[2].IsPublic.ShouldBeFalse();
        }
    }
}