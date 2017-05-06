using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using paramore.brighter.commandprocessor;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class DictioanriesControllerTests
    {
        private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
        private readonly FakeQueryProcessor _fakeQueryProcessor;
        private readonly FakeDictionaryRenderer _fakeDictionaryRenderer;
        private readonly FakeUserHelper _fakeUserHelper;

        readonly DictionariesController _controller;

        public DictioanriesControllerTests()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            _mockCommandProcessor = new Mock<IAmACommandProcessor>();
            _fakeQueryProcessor = new FakeQueryProcessor();

            var fakeDictionariesRenderer = new FakeDictionariesRenderer();
            _fakeDictionaryRenderer = new FakeDictionaryRenderer();
            _fakeUserHelper = new FakeUserHelper();

            _controller = new DictionariesController(_mockCommandProcessor.Object, _fakeQueryProcessor, _fakeUserHelper, fakeDictionariesRenderer, _fakeDictionaryRenderer);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionariesPublicUser()
        {
            _fakeQueryProcessor.SetupResultFor<DictionariesByUserQuery, IEnumerable<Dictionary>>(new Collection<Dictionary>());

            var result =  _controller.Get().Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionariesByUserQuery>(q => q.UserId == null);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionatiresForUser()
        {
            var userId = "user1234";
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionariesByUserQuery, IEnumerable<Dictionary>>(new Collection<Dictionary>());

            var result = _controller.Get().Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionariesByUserQuery>(q => q.UserId == userId);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionaryAsPublicUser()
        {
            var dictionaryId = 2332;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Get(dictionaryId).Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionaryByIdQuery>(q => q.UserId == null && q.DictionaryId == dictionaryId);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionaryForUser()
        {
            var userId = "user1234";
            var dictionaryId = 2332;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Get(dictionaryId).Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionaryByIdQuery>(q => q.UserId == userId && q.DictionaryId == dictionaryId);
        }


        [Fact]
        public void WhenDitionaryNotFound_ShouldReturnNotFoundResult()
        {
            var result = _controller.Get(12).Result;

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void WhenPosted_ShouldAddToDictionary()
        {
            var userId = "user1234";
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new DictionaryView
            {
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Post(dictionaryView).Result as CreatedResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Location);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<AddDictionaryCommand>(), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }

        [Fact]
        public void WhenPostNameDoesNotExist_ShouldReturnBadRequest()
        {
            var dictionaryView = new DictionaryView
            {
                Language = 23,
                IsPublic = true,
                Name = ""
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Post(dictionaryView).Result;

            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public void WhenPut_ShouldRaiseCommandToUpdateDictionary()
        {
            var userId = "user1234";
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new DictionaryView
            {
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<NoContentResult>(result);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<UpdateDictionaryCommand>(), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<UpdateDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }
       
        [Fact]
        public void WhenPutNonExistingDictionary_ShouldCreateDictionary()
        {
            var userId = "user1234";
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);

            var dictionaryView = new DictionaryView
            {
                Id = dictionaryId,
                Language = 23,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<CreatedResult>(result);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<AddDictionaryCommand>(), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false, default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }

        [Fact]
        public void WhenPutNameDoesNotExist_ShouldReturnBadRequest()
        {
            var dictionaryId = 344;
            var dictionaryView = new DictionaryView
            {
                Id = dictionaryId,
                Language = 23,
                IsPublic = true,
                Name = ""
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public void WhenDeleted_ShouldRemoveDictionary()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Delete(dictionaryId).Result;

            Assert.IsType<NoContentResult>(result);

            _mockCommandProcessor.Verify(x => x.Send(It.IsAny<DeleteDictionaryCommand>()));
            _mockCommandProcessor.Verify(x => x.Send(It.Is<DeleteDictionaryCommand>(c => c.DictionaryId == dictionaryId)));
        }

        [Fact]

        public void WhenDeletingNonExistingDictionary_ShouldReturnNotFound()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);

            var result = _controller.Delete(dictionaryId).Result;

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
