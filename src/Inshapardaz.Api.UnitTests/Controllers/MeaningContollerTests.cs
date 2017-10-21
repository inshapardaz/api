using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingMeaningsForAWord : MeaningControllerTestContext
    {
        public WhenGettingMeaningsForAWord()
        {
            FakeQueryProcessor.SetupResultFor<WordMeaningByWordQuery, IEnumerable<Meaning>>(new List<Meaning>());
            Result = Controller.GetMeaningForWord(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnListOfMeaning()
        {
            var result = Result as ObjectResult;

            Assert.IsType<List<MeaningView>>(result.Value);
        }
    }

    public class WhenGettingMeaningByWordDetailThatDoesNotExist : MeaningControllerTestContext
    {
        public WhenGettingMeaningByWordDetailThatDoesNotExist()
        {
            FakeQueryProcessor.SetupResultFor<WordMeaningByWordQuery, IEnumerable<Meaning>>(new List<Meaning>());
            Result = Controller.GetMeaningForWord(0, 9).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }
    }

    public class WhenGettingMeaningByById : MeaningControllerTestContext
    {
        public WhenGettingMeaningByById()
        {
            FakeQueryProcessor.SetupResultFor<WordMeaningByIdQuery, Meaning>(new Meaning());
            FakeMeaningRenderer.WithView(new MeaningView());
            Result = Controller.Get(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnResultOfMeaning()
        {
            var result = Result as ObjectResult;
            Assert.IsType<MeaningView>(result.Value);
        }
    }

    public class WhenGettingMeaningByIdThatDoesNotExist : MeaningControllerTestContext
    {
        public WhenGettingMeaningByIdThatDoesNotExist()
        {
            Result = Controller.Get(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenGettingMeaningByIdForPrivateDictionaryOfOtherUsers : MeaningControllerTestContext
    {
        public WhenGettingMeaningByIdForPrivateDictionaryOfOtherUsers()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            Result = Controller.Get(0, 23).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingMeaning : MeaningControllerTestContext
    {
        public WhenAddingMeaning()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            FakeMeaningRenderer.WithView(new MeaningView());
            FakeMeaningRenderer.WithLink("self", new Uri("http://link.test/123"));

            Result = Controller.Post(0, 23, new MeaningView()).Result;
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
            Assert.IsType<MeaningView>(response.Value);
        }
    }

    public class WhenAddingMeaningToNonExistantWordDetail : MeaningControllerTestContext
    {
        public WhenAddingMeaningToNonExistantWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Post(0, 23, new MeaningView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequst()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenAddingMeaningToDictionaryWithNoWriteAccess : MeaningControllerTestContext
    {
        public WhenAddingMeaningToDictionaryWithNoWriteAccess()
        {
            UserHelper.AsReader();
            Result = Controller.Post(0, 23, new MeaningView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenUpdatingAMeaning : MeaningControllerTestContext
    {
        public WhenUpdatingAMeaning()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordMeaningByIdQuery, Meaning>(new Meaning());
            Result = Controller.Put(0, 32, new MeaningView()).Result;
        }

        [Fact]
        public void ShouldReturnNoContentResult()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenUpdatingANonExistingMeaning : MeaningControllerTestContext
    {
        public WhenUpdatingANonExistingMeaning()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Put(0, 32, new MeaningView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequestResult()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenUpdatingAMeaningInDictionaryUserHasNoWriteAccess : MeaningControllerTestContext
    {
        public WhenUpdatingAMeaningInDictionaryUserHasNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<WordMeaningByIdQuery, Meaning>(new Meaning());
            Result = Controller.Put(0, 32, new MeaningView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenDeleteingAMeaning : MeaningControllerTestContext
    {
        public WhenDeleteingAMeaning()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<WordMeaningByIdQuery, Meaning>(new Meaning());
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnNoContent()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenDeleteingNonExisitngMeaning : MeaningControllerTestContext
    {
        public WhenDeleteingNonExisitngMeaning()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenDeleteingAMeaningFromDictionaryWithNoWriteAccess : MeaningControllerTestContext
    {
        public WhenDeleteingAMeaningFromDictionaryWithNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByMeaningIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<WordMeaningByIdQuery, Meaning>(new Meaning());
            Result = Controller.Delete(0, 34).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public abstract class MeaningControllerTestContext
    {
        protected readonly MeaningController Controller;
        protected readonly Mock<IAmACommandProcessor> MockCommandProcessor;
        protected readonly FakeMeaningRenderer FakeMeaningRenderer;
        protected readonly FakeQueryProcessor FakeQueryProcessor;
        protected readonly FakeUserHelper UserHelper;
        protected IActionResult Result;

        protected MeaningControllerTestContext()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            MockCommandProcessor = new Mock<IAmACommandProcessor>();
            FakeMeaningRenderer = new FakeMeaningRenderer();
            FakeQueryProcessor = new FakeQueryProcessor();
            UserHelper = new FakeUserHelper();
            Controller = new MeaningController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper, FakeMeaningRenderer);
        }
    }
}