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

    public class WhenGettingDetailsByIdThatDoesNotExist : WordDetailControllerTestContext
    {
        public WhenGettingDetailsByIdThatDoesNotExist()
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

    public class WhenGettingDetailsByIdForPrivateDictionaryOfOtherUsers : WordDetailControllerTestContext
    {
        public WhenGettingDetailsByIdForPrivateDictionaryOfOtherUsers()
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
            MockCommandProcessor = new Mock<IAmACommandProcessor>();
            FakeWordDetailRenderer = new FakeWordDetailRenderer();
            FakeQueryProcessor = new FakeQueryProcessor();
            UserHelper = new FakeUserHelper();
            Controller = new WordDetailController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper,FakeWordDetailRenderer);
        }
    }
}
