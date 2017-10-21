using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class DictionariesControllerTests
    {
        private readonly Mock<IAmACommandProcessor> _mockCommandProcessor;
        private readonly FakeQueryProcessor _fakeQueryProcessor;
        private readonly FakeDictionaryRenderer _fakeDictionaryRenderer;
        private readonly FakeUserHelper _fakeUserHelper;

        private readonly DictionariesController _controller;

        public DictionariesControllerTests()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            _mockCommandProcessor = new Mock<IAmACommandProcessor>();
            _fakeQueryProcessor = new FakeQueryProcessor();

            _fakeDictionaryRenderer = new FakeDictionaryRenderer();
            _fakeUserHelper = new FakeUserHelper();

            _controller = new DictionariesController(_mockCommandProcessor.Object);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionariesPublicUser()
        {
            _fakeQueryProcessor.SetupResultFor<DictionariesByUserQuery, IEnumerable<Dictionary>>(
                new Collection<Dictionary>());

            var result = _controller.GetDictionaries().Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionariesByUserQuery>(q => q.UserId == Guid.Empty);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionariesForUser()
        {
            var userId = Guid.NewGuid();
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionariesByUserQuery, IEnumerable<Dictionary>>(
                new Collection<Dictionary>());

            var result = _controller.GetDictionaries().Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionariesByUserQuery>(q => q.UserId == userId);
        }

        [Fact]
        public void WhenAnonymousCall_ShouldQueryDictionaryAsPublicUser()
        {
            var dictionaryId = 2332;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.GetDictionaryById(dictionaryId).Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionaryByIdQuery>(
                q => q.UserId == Guid.Empty && q.DictionaryId == dictionaryId);
        }

        [Fact]
        public void WhenAuthenticatedCall_ShouldQueryDictionaryForUser()
        {
            var userId = Guid.NewGuid();
            var dictionaryId = 2332;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.GetDictionaryById(dictionaryId).Result;

            _fakeQueryProcessor.ShouldHaveExecuted<DictionaryByIdQuery>(
                q => q.UserId == userId && q.DictionaryId == dictionaryId);
        }

        [Fact]
        public void WhenDictionaryNotFound_ShouldReturnNotFoundResult()
        {
            var result = _controller.GetDictionaryById(12).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void WhenPosted_ShouldAddToDictionary()
        {
            var userId = Guid.NewGuid();
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new DictionaryView
            {
                Language = Languages.Persian,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Post(dictionaryView).Result as CreatedResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Location);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<AddDictionaryCommand>(), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }

        [Fact]
        public void WhenPostNameDoesNotExist_ShouldReturnBadRequest()
        {
            var dictionaryView = new DictionaryView
            {
                Language = Languages.Portuguese,
                IsPublic = true,
                Name = "sdsd"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Post(dictionaryView).Result;

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void WhenPut_ShouldRaiseCommandToUpdateDictionary()
        {
            var userId = Guid.NewGuid();
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            var dictionaryView = new DictionaryView
            {
                Language = Languages.Persian,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<NoContentResult>(result);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<UpdateDictionaryCommand>(), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<UpdateDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<UpdateDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<UpdateDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }

        [Fact]
        public void WhenPutNonExistingDictionary_ShouldCreateDictionary()
        {
            var userId = Guid.NewGuid();
            var dictionaryId = 344;
            _fakeUserHelper.WithUserId(userId);
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);

            var dictionaryView = new DictionaryView
            {
                Id = dictionaryId,
                Language = Languages.Sanskrit,
                IsPublic = true,
                Name = "test dictionary"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<CreatedResult>(result);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<AddDictionaryCommand>(), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.Name == dictionaryView.Name), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.Language == dictionaryView.Language), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.IsPublic == dictionaryView.IsPublic), false,
                default(CancellationToken)));
            _mockCommandProcessor.Verify(x => x.SendAsync(
                It.Is<AddDictionaryCommand>(d => d.Dictionary.UserId == userId), false, default(CancellationToken)));
        }

        [Fact]
        public void WhenPutNameDoesNotExist_ShouldReturnBadRequest()
        {
            var dictionaryId = 344;
            var dictionaryView = new DictionaryView
            {
                Id = dictionaryId,
                Language = Languages.Persian,
                IsPublic = true,
                Name = "sdsdsdsd"
            };
            _fakeDictionaryRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            var result = _controller.Put(dictionaryId, dictionaryView).Result;

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void WhenDeleted_ShouldRemoveDictionary()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.Delete(dictionaryId).Result;

            Assert.IsType<NoContentResult>(result);

            _mockCommandProcessor.Verify(x => x.SendAsync(It.IsAny<DeleteDictionaryCommand>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
            _mockCommandProcessor.Verify(x => x.SendAsync(It.Is<DeleteDictionaryCommand>(c => c.DictionaryId == dictionaryId), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public void WhenDeletingNonExistingDictionary_ShouldReturnNotFound()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);

            var result = _controller.Delete(dictionaryId).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void WhenCreatingDownloadForDictionary_ShouldReturnCreated()
        {
            const int dictionaryId = 23;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(new Dictionary());

            var result = _controller.CreateDownloadForDictionary(dictionaryId).Result;

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void WhenCreatingDownloadForDictionaryAndDictionaryNotFound_ShouldReturnCreated()
        {
            const int dictionaryId = 43;
            _fakeQueryProcessor.SetupResultFor<DictionaryByIdQuery, Dictionary>(null);

            var result = _controller.CreateDownloadForDictionary(dictionaryId).Result;

            Assert.IsType<NotFoundResult>(result);
        }
    }
}