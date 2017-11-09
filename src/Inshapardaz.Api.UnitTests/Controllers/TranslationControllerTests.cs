using AutoMapper;
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
    public class WhenGettingTranslationsForWord : TranslationControllerTestContext
    {
        public WhenGettingTranslationsForWord()
        {
            var translations = new List<Translation>
            {
                new Translation()
            };
            FakeQueryProcessor.SetupResultFor<GetTranslationsByWordIdQuery, IEnumerable<Translation>>(translations);
            FakeTranslationRenderer.WithView(new TranslationView());
            Result = Controller.GetTranslationForWord(1, 3).Result;
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
            FakeQueryProcessor.SetupResultFor<GetTranslationsByWordIdQuery, IEnumerable<Translation>>(new List<Translation>());
            Result = Controller.GetTranslationForWord(1, 9).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            Result = Controller.GetTranslationForWord(1, 9).Result;
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
            FakeQueryProcessor.SetupResultFor<GetTranslationsByLanguageQuery, IEnumerable<Translation>>(translations);
            FakeTranslationRenderer.WithView(new TranslationView());
            Result = Controller.GetTranslationForWord(1, 3, Languages.English).Result;
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
            FakeQueryProcessor.SetupResultFor<GetTranslationsByLanguageQuery, IEnumerable<Translation>>(new List<Translation>());
            Result = Controller.GetTranslationForWord(1, 9, Languages.French).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            Result = Controller.GetTranslationForWord(1, 9, Languages.Hindi).Result;
        }

        [Fact]
        public void ShouldReturnUnAuthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingTranslation : TranslationControllerTestContext
    {
        public WhenAddingTranslation()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(new Word());
            FakeTranslationRenderer.WithView(new TranslationView());

            Result = Controller.Post(1, 23, new TranslationView()).Result;
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
            Assert.IsType<TranslationView>(response.Value);
        }
    }

    public class WhenAddingTranslationToNonExistantWordDetail : TranslationControllerTestContext
    {
        public WhenAddingTranslationToNonExistantWordDetail()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Post(1, 23, new TranslationView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequst()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenAddingTranslationToDictionaryWithNoWriteAccess : TranslationControllerTestContext
    {
        public WhenAddingTranslationToDictionaryWithNoWriteAccess()
        {
            UserHelper.AsReader();
            Result = Controller.Post(1, 23, new TranslationView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenUpdatingATranslation : TranslationControllerTestContext
    {
        public WhenUpdatingATranslation()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetTranslationByIdQuery, Translation>(new Translation());
            Result = Controller.Put(1, 32, new TranslationView()).Result;
        }

        [Fact]
        public void ShouldReturnNoContentResult()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenUpdatingANonExistingTranslation : TranslationControllerTestContext
    {
        public WhenUpdatingANonExistingTranslation()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Put(1, 32, new TranslationView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequestResult()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenUpdatingATranslationInDictionaryUserHasNoWriteAccess : TranslationControllerTestContext
    {
        public WhenUpdatingATranslationInDictionaryUserHasNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<GetWordMeaningByIdQuery, Meaning>(new Meaning());
            Result = Controller.Put(1, 32, new TranslationView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenDeleteingATransaltion : TranslationControllerTestContext
    {
        public WhenDeleteingATransaltion()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetTranslationByIdQuery, Translation>(new Translation());
            Result = Controller.Delete(1, 34).Result;
        }

        [Fact]
        public void ShouldReturnNoContent()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenDeleteingNonExisitngTranslation : TranslationControllerTestContext
    {
        public WhenDeleteingNonExisitngTranslation()
        {
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Delete(1, 34).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenDeleteingATranslationFromDictionaryWithNoWriteAccess : TranslationControllerTestContext
    {
        public WhenDeleteingATranslationFromDictionaryWithNoWriteAccess()
        {
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<GetDictionaryByIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<GetTranslationByIdQuery, Translation>(new Translation());
            Result = Controller.Delete(1, 34).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
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
            Controller = new TranslationController(MockCommandProcessor.Object);
        }
    }
}