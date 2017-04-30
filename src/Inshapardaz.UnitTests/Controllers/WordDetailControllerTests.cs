using System.Collections.Generic;
using AutoMapper;
using Inshapardaz.Api.Configuration;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Model;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using paramore.brighter.commandprocessor;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingWordDetailsByWord : WordDetailControllerTestContext
    {
        public WhenGettingWordDetailsByWord()
        {
            UserHelper.WithUserId("56");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "56" });
            FakeQueryProcessor.SetupResultFor<WordDetailsByWordQuery, IEnumerable<WordDetail>>(new List<WordDetail>{ new WordDetail() });
            Result = Controller.GetForWord(9).Result;
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
            UserHelper.WithUserId("56");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "56" });
            Result = Controller.GetForWord(9).Result;
        }

        [Fact]
        public void ShouldReturnNotFoundResult()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenGettingWordDetailsByWordThatUserHasNotAllowedAccess : WordDetailControllerTestContext
    {
        public WhenGettingWordDetailsByWordThatUserHasNotAllowedAccess()
        {
            UserHelper.WithUserId("56");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "23"});
            Result = Controller.GetForWord(9).Result;
        }

        [Fact]
        public void ShouldReturnUnAuthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenGettingWordDetailsById : WordDetailControllerTestContext
    {

        public WhenGettingWordDetailsById()
        {
            UserHelper.WithUserId("12");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "12" });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Get(23).Result;
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
            UserHelper.WithUserId("12");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "12" });
            Result = Controller.Get(23).Result;
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
            UserHelper.WithUserId("123");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "12" });
            Result = Controller.Get(23).Result;
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
            UserHelper.WithUserId("23");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "23" });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            FakeWordDetailRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            Result = Controller.Post(23, new WordDetailView()).Result;
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
            UserHelper.WithUserId("23");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "23" });
            Result = Controller.Post(23, new WordDetailView()).Result;
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
            Result = Controller.Post(23, new WordDetailView()).Result;
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
            Result = Controller.Post(23, null).Result;
        }

        [Fact]
        public void ShouldReturnBadRequest()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenUpdatingAWordDetail : WordDetailControllerTestContext
    {
        public WhenUpdatingAWordDetail()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Put(32, new WordDetailView()).Result;
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
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            Result = Controller.Put(32, new WordDetailView()).Result;
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
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "32" });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Put(32, new WordDetailView()).Result;
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
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Delete(34).Result;
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
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            Result = Controller.Delete(34).Result;
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
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "32" });
            FakeQueryProcessor.SetupResultFor<WordDetailByIdQuery, WordDetail>(new WordDetail());
            Result = Controller.Delete(34).Result;
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
            Controller = new WordDetailController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper,FakeWordDetailRenderer);
        }
    }
}
