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
using Inshapardaz.Domain.Commands;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Inshapardaz.Configuration;

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
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

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


        [Fact]
        public void WhenDitionaryNotFound_ShouldReturnNotFoundResult()
        {
            var result = _controller.Get(12);

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void WhenPosted_ShouldRaiseCommandToAddDictionary()
        {
            var userId = "user1234";
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new Model.DictionaryView
            {
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Post(dictionaryView) as CreatedResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Location);

            _mockCommandProcessor.Verify(x => x.Send(It.IsAny<AddDictionaryCommand>()));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId)));
        }

        [Fact]
        public void WhenPut_ShouldRaiseCommandToUpdateDictionary()
        {
            var userId = "user1234";
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new Model.DictionaryView
            {
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(new Dictionary()));

            var result = _controller.Put(dictionaryId, dictionaryView);

            Assert.IsType<OkResult>(result);

            _mockCommandProcessor.Verify(x => x.Send(It.IsAny<UpdateDictionaryCommand>()));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.UserId == userId)));
        }
       
        [Fact]
         public void WhenPutNonExistingDictionary_ShouldCreateDictionary()
        {
            var userId = "user1234";
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(null));

            var dictionaryView = new Model.DictionaryView
            {
                Id = dictionaryId,
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Put(dictionaryId, dictionaryView);

            Assert.IsType<CreatedResult>(result);

            _mockCommandProcessor.Verify(x => x.Send(It.IsAny<AddDictionaryCommand>()));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic)));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId)));
        }

        [Fact]

        public void WhenDeleted_ShouldRemoveDictionary()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(new Dictionary()));

            var result = _controller.Delete(dictionaryId);

            Assert.IsType<NoContentResult>(result);

            _mockCommandProcessor.Verify(x => x.Send(It.IsAny<DeleteDictionaryCommand>()));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<DeleteDictionaryCommand>(c => c.DictionaryId == dictionaryId)));
        }

        [Fact]

        public void WhenDeletingNonExistingDictionary_ShouldReturnNotFound()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, GetDictionaryByIdQuery.Response>(new GetDictionaryByIdQuery.Response(null));

            var result = _controller.Delete(dictionaryId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
