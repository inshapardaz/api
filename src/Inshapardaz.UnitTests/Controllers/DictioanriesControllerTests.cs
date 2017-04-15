using System.Collections.ObjectModel;
using Moq;
using paramore.brighter.commandprocessor;

using Xunit;
using Inshapardaz.Controllers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.UnitTests.Fakes;
using Inshapardaz.UnitTests.Fakes.Helpers;
using Inshapardaz.UnitTests.Fakes.Renderers;

namespace Inshapardaz.UnitTests.Controllers
{
    public class DictioanriesControllerTests
    {
        private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
        private readonly FakeQueryProcessor _fakeQueryProcessor;
        private readonly FakeDictionariesRenderer _fakeDictionariesRenderer;
        private readonly FakeDictionaryRenderer _fakeDictionaryRenderer;
        private readonly FakeUserHelper _fakeUserHelper;

        readonly DictionariesController _controller;

        public DictioanriesControllerTests()
        {
            _mockCommandProcessor = new Mock<IAmACommandProcessor>();
            _fakeQueryProcessor = new FakeQueryProcessor();

            _fakeDictionariesRenderer = new FakeDictionariesRenderer();
            _fakeDictionaryRenderer = new FakeDictionaryRenderer();
            _fakeUserHelper = new FakeUserHelper();

            _controller = new DictionariesController(_mockCommandProcessor.Object, _fakeQueryProcessor, _fakeUserHelper, _fakeDictionariesRenderer, _fakeDictionaryRenderer);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionariesPublicUser()
        {
            _fakeQueryProcessor.SetupResultFor<GetDictionariesByUserQuery, GetDictionariesByUserQuery.Response>(new GetDictionariesByUserQuery.Response(new Collection<Dictionary>()));

            _controller.Get();

            _fakeQueryProcessor.ShouldHaveExecuted<GetDictionariesByUserQuery>(q => q.UserId == null);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionatiresForUser()
        {
            var userId = "user1234";
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<GetDictionariesByUserQuery, GetDictionariesByUserQuery.Response>(new GetDictionariesByUserQuery.Response(new Collection<Dictionary>()));

            _controller.Get();

            _fakeQueryProcessor.ShouldHaveExecuted<GetDictionariesByUserQuery>(q => q.UserId == userId);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionaryAsPublicUser()
        {
            var dictionaryId = 2332;
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(new Dictionary()));

            _controller.Get(dictionaryId);

            _fakeQueryProcessor.ShouldHaveExecuted<GetDictionaryByIdQuery>(q => q.UserId == null && q.DictionaryId == dictionaryId);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionaryForUser()
        {
            var userId = "user1234";
            var dictionaryId = 2332;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(new Dictionary()));

            _controller.Get(dictionaryId);

            _fakeQueryProcessor.ShouldHaveExecuted<GetDictionaryByIdQuery>(q => q.UserId == userId && q.DictionaryId == dictionaryId);
        }
    }
}
