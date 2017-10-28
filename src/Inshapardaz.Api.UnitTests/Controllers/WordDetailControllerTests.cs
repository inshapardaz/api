﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingWordDetailsByWord : WordDetailControllerTestContext
    {
        public WhenGettingWordDetailsByWord()
        {
            FakeQueryProcessor.SetupResultFor<WordDetailsByWordQuery, IEnumerable<WordDetail>>(new List<WordDetail> { new WordDetail() });
            Result = Controller.GetDetailsForWord(0, 9).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnListOfWordDetails()
        {
            var result = Result as ObjectResult;
            Assert.IsType<List<WordDetailView>>(result.Value);
        }
    }

    public class WhenGettingWordDetailsByWordThatDoesNotExist : WordDetailControllerTestContext
    {
        public WhenGettingWordDetailsByWordThatDoesNotExist()
        {
            FakeQueryProcessor.SetupResultFor<WordDetailsByWordQuery, IEnumerable<WordDetail>>(new List<WordDetail>());
            Result = Controller.GetDetailsForWord(0, 9).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }
    }

    public class WhenGettingWordDetailsById : WordDetailControllerTestContext
    {
        public WhenGettingWordDetailsById()
        {
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.GetWordDetailById(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnResultOfWordDetail()
        {
            var result = Result as ObjectResult;
            Assert.IsType<WordDetailView>(result.Value);
        }
    }

    public class WhenGettingDetailByIdThatDoesNotExist : WordDetailControllerTestContext
    {
        public WhenGettingDetailByIdThatDoesNotExist()
        {
            Result = Controller.GetWordDetailById(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenGettingDetailByIdForPrivateDictionaryOfOtherUsers : WordDetailControllerTestContext
    {
        public WhenGettingDetailByIdForPrivateDictionaryOfOtherUsers()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            Result = Controller.GetWordDetailById(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingWordDetails : WordDetailControllerTestContext
    {
        public WhenAddingWordDetails()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            FakeWordDetailRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            Result = Controller.Post(0, 23, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnCreated()
        {
            Assert.IsType<CreatedResult>(Result);
        }

        [Fact]
        public void ShouldReturnNewlyCreatedWordDetailLink()
        {
            var response = Result as CreatedResult;

            Assert.NotNull(response.Location);
        }

        [Fact]
        public void ShouldReturnCreatedWordDetail()
        {
            var response = Result as CreatedResult;

            Assert.NotNull(response.Value);
            Assert.IsType<WordDetailView>(response.Value);
        }
    }

    public class WhenAddingWordDetailsToNonExistantWord : WordDetailControllerTestContext
    {
        public WhenAddingWordDetailsToNonExistantWord()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Post(0, 23, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequst()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenAddingWordDetailsToDictionaryWithNoWriteAccess : WordDetailControllerTestContext
    {
        public WhenAddingWordDetailsToDictionaryWithNoWriteAccess()
        {
            UserHelper.AsReader();
            Result = Controller.Post(0, 23, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingWordDetailWithoutObjectInBody : WordDetailControllerTestContext
    {
        public WhenAddingWordDetailWithoutObjectInBody()
        {
            Result = Controller.Post(0, 23, null).Result;
        }

        [Fact]
        public void ShouldReturnBadRequest()
        {
            Assert.IsType<BadRequestObjectResult>(Result);
        }
    }

    public class WhenUpdatingAWordDetail : WordDetailControllerTestContext
    {
        public WhenUpdatingAWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Put(0, 32, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnNoContentResult()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenUpdatingANonExistingWordDetail : WordDetailControllerTestContext
    {
        public WhenUpdatingANonExistingWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Put(0, 32, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequestResult()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenUpdatingAWordDetailInDictionaryUserHasNoWriteAccess : WordDetailControllerTestContext
    {
        public WhenUpdatingAWordDetailInDictionaryUserHasNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Put(0, 32, new WordDetailView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenDeleteingAWordDetail : WordDetailControllerTestContext
    {
        public WhenDeleteingAWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnNoContent()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenDeleteingNonExisitngWordDetail : WordDetailControllerTestContext
    {
        public WhenDeleteingNonExisitngWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenDeleteingAWordDetailFromDictionaryWithNoWriteAccess : WordDetailControllerTestContext
    {
        public WhenDeleteingAWordDetailFromDictionaryWithNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public abstract class WordDetailControllerTestContext
    {
        protected readonly WordDetailController Controller;
        protected readonly Mock<IAmACommandProcessor> MockCommandProcessor;
        protected readonly FakeWordDetailRenderer FakeWordDetailRenderer;
        protected readonly FakeQueryProcessor FakeQueryProcessor;
        protected readonly FakeUserHelper UserHelper;
        protected IActionResult Result;

        protected WordDetailControllerTestContext()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            MockCommandProcessor = new Mock<IAmACommandProcessor>();
            FakeWordDetailRenderer = new FakeWordDetailRenderer();
            FakeQueryProcessor = new FakeQueryProcessor();
            UserHelper = new FakeUserHelper();
            Controller = new WordDetailController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper, FakeWordDetailRenderer);
        }
    }
}