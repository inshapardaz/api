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
using System.Collections.Generic;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingTranslationsForWord : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForWord()
        {
            var translations = new List<Translation>
            {
                new Translation()
            };
            FakeQueryProcessor.SetupResultFor<TranslationsByWordIdQuery, IEnumerable<Translation>>(translations);
            FakeTranslationRenderer.WithView(new TranslationView());
            Result = Controller.GetTranslationForWord(3).Result;
        }

        [Fact]
        public void ShouldReturnokResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnListOfTranslations()
        {
            var result = Result as ObjectResult;

            Assert.IsType<List<TranslationView>>(result.Value);
        }
    }

    public class WhenGettingTranslationsForWordThatDoesNotExist : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForWordThatDoesNotExist()
        {
            FakeQueryProcessor.SetupResultFor<TranslationsByWordIdQuery, IEnumerable<Translation>>(new List<Translation>());
            Result = Controller.GetTranslationForWord(9).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }
    }

    public class WhenGettingTranslationsForWordThatUserHasNotAllowedAccess : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForWordThatUserHasNotAllowedAccess()
        {
            UserHelper.WithUserId("56");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "23" });
            Result = Controller.GetTranslationForWord(9).Result;
        }

        [Fact]
        public void ShouldReturnUnAuthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenGettingTranslationsForLanguage : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForLanguage()
        {
            var translations = new List<Translation>
            {
                new Translation()
            };
            FakeQueryProcessor.SetupResultFor<TranslationsByLanguageQuery, IEnumerable<Translation>>(translations);
            FakeTranslationRenderer.WithView(new TranslationView());
            Result = Controller.GetTranslationForWord(3, Languages.English).Result;
        }

        [Fact]
        public void ShouldReturnokResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }

        [Fact]
        public void ShouldReturnListOfTranslations()
        {
            var result = Result as ObjectResult;

            Assert.IsType<List<TranslationView>>(result.Value);
        }
    }

    public class WhenGettingTranslationsForLanguageThatDoesNotExist : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForLanguageThatDoesNotExist()
        {
            FakeQueryProcessor.SetupResultFor<TranslationsByLanguageQuery, IEnumerable<Translation>>(new List<Translation>());
            Result = Controller.GetTranslationForWord(9, Languages.French).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }
    }

    public class WhenGettingTranslationsForLanguageThatUserHasNotAllowedAccess : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForLanguageThatUserHasNotAllowedAccess()
        {
            UserHelper.WithUserId("56");
            FakeQueryProcessor.SetupResultFor<GetDictionaryByWordDetailIdQuery, Dictionary>(new Dictionary { UserId = "23" });
            Result = Controller.GetTranslationForWord(9, Languages.Hindi).Result;
        }

        [Fact]
        public void ShouldReturnUnAuthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public abstract class TranslationControllerTestContext
    {
        protected readonly TranslationController Controller;
        protected readonly Mock<IAmACommandProcessor> MockCommandProcessor;
        protected readonly FakeTranslationRenderer FakeTranslationRenderer;
        protected readonly FakeQueryProcessor FakeQueryProcessor;
        protected readonly FakeUserHelper UserHelper;
        protected IActionResult Result;

        protected TranslationControllerTestContext()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            MockCommandProcessor = new Mock<IAmACommandProcessor>();
            FakeTranslationRenderer = new FakeTranslationRenderer();
            FakeQueryProcessor = new FakeQueryProcessor();
            UserHelper = new FakeUserHelper();
            Controller = new TranslationController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper, FakeTranslationRenderer);
        }
    }
}