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
    public class WhenGettingRelationshipsByWord : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipsByWord()
        {
            FakeQueryProcessor.SetupResultFor<RelationshipByWordIdQuery, IEnumerable<WordRelation>>(new List<WordRelation> { new WordRelation() });
            FakeRelationshipRenderer.WithView(new RelationshipView());
            Result = Controller.GetRelationshipForWord(9).Result;
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
            Assert.IsType<List<RelationshipView>>(result.Value);
        }
    }

    public class WhenGettingRelationshipsByWordThatDoesNotExist : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipsByWordThatDoesNotExist()
        {
            FakeQueryProcessor.SetupResultFor<RelationshipByWordIdQuery, IEnumerable<WordRelation>>(new List<WordRelation>());
            Result = Controller.GetRelationshipForWord(9).Result;
        }

        [Fact]
        public void ShouldReturnOkResult()
        {
            Assert.IsType<OkObjectResult>(Result);
        }
    }

    public class WhenGettingRelationshipById : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipById()
        {
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            FakeRelationshipRenderer.WithView(new RelationshipView());
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
            Assert.IsType<RelationshipView>(result.Value);
        }
    }

    public class WhenGettingRelationshipByIdThatDoesNotExist : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipByIdThatDoesNotExist()
        {
            Result = Controller.Get(23).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenGettingRelationshipByIdForPrivateDictionaryOfOtherUsers : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipByIdForPrivateDictionaryOfOtherUsers()
        {
            UserHelper.WithUserId("123");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "12" });
            Result = Controller.Get(23).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingRelation : RelationshipControllerTestContext
    {
        public WhenAddingRelation()
        {
            UserHelper.WithUserId("23");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "23" });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            FakeRelationshipRenderer.WithView(new RelationshipView());
            FakeRelationshipRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            Result = Controller.Post(23, new RelationshipView()).Result;
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
            Assert.IsType<RelationshipView>(response.Value);
        }
    }

    public class WhenAddingRelationshipsForNonExistantWord : RelationshipControllerTestContext
    {
        public WhenAddingRelationshipsForNonExistantWord()
        {
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(w => w.Id == 12, new Word());
            Result = Controller.Post(23, new RelationshipView { RelatedWordId = 12 }).Result;
        }

        [Fact]
        public void ShouldReturnBadRequst()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenAddingRelationshipsToNonExistantWord : RelationshipControllerTestContext
    {
        public WhenAddingRelationshipsToNonExistantWord()
        {
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(w => w.Id == 23, new Word());
            Result = Controller.Post(23, new RelationshipView { RelatedWordId = 12 }).Result;
        }

        [Fact]
        public void ShouldReturnBadRequst()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenAddingRelationshipToDictionaryWithNoWriteAccess : RelationshipControllerTestContext
    {
        public WhenAddingRelationshipToDictionaryWithNoWriteAccess()
        {
            UserHelper.WithUserId("2");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "32" });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());

            Result = Controller.Post(23, new RelationshipView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenAddingRelationshipToWordInAnotherDictionary : RelationshipControllerTestContext
    {
        public WhenAddingRelationshipToWordInAnotherDictionary()
        {
            UserHelper.WithUserId("23");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(x => x.WordId == 23, new Dictionary { UserId = "23" });
            FakeQueryProcessor.SetupResultFor<WordByIdQuery, Word>(new Word());

            Result = Controller.Post(23, new RelationshipView { RelatedWordId = 45 }).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenAddingRelationshipWithoutBody : RelationshipControllerTestContext
    {
        public WhenAddingRelationshipWithoutBody()
        {
            Result = Controller.Post(23, null).Result;
        }

        [Fact]
        public void ShouldReturnBadRequest()
        {
            Assert.IsType<BadRequestObjectResult>(Result);
        }
    }

    public class WhenUpdatingARelationship : RelationshipControllerTestContext
    {
        public WhenUpdatingARelationship()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(32, new RelationshipView()).Result;
        }

        [Fact]
        public void ShouldReturnNoContentResult()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenUpdatingANonExistingRelationship : RelationshipControllerTestContext
    {
        public WhenUpdatingANonExistingRelationship()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            Result = Controller.Put(32, new RelationshipView()).Result;
        }

        [Fact]
        public void ShouldReturnBadRequestResult()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenUpdatingARelationshipWithTargetWordMissing : RelationshipControllerTestContext
    {
        public WhenUpdatingARelationshipWithTargetWordMissing()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(q => q.Id == 32, new WordRelation());
            Result = Controller.Put(32, new RelationshipView { RelatedWordId = 344 }).Result;
        }

        [Fact]
        public void ShouldReturnBadRequestResult()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenUpdatingRelationshipInDictionaryUserHasNoWriteAccess : RelationshipControllerTestContext
    {
        public WhenUpdatingRelationshipInDictionaryUserHasNoWriteAccess()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "32" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(32, new RelationshipView()).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorisedResult()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public class WhenUpdatingRelationshipWithWordFromOtherDictionary : RelationshipControllerTestContext
    {
        public WhenUpdatingRelationshipWithWordFromOtherDictionary()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(q => q.WordId == 32, new Dictionary { Id = 3, UserId = "33" });
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(q => q.WordId == 43, new Dictionary { Id = 4, UserId = "33" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(32, new RelationshipView { SourceWordId = 32, RelatedWordId = 43 }).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorisedResult()
        {
            Assert.IsType<BadRequestResult>(Result);
        }
    }

    public class WhenDeleteingRelationship : RelationshipControllerTestContext
    {
        public WhenDeleteingRelationship()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnNoContent()
        {
            Assert.IsType<NoContentResult>(Result);
        }
    }

    public class WhenDeleteingNonExisitngRelationship : RelationshipControllerTestContext
    {
        public WhenDeleteingNonExisitngRelationship()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "33" });
            Result = Controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnNotFound()
        {
            Assert.IsType<NotFoundResult>(Result);
        }
    }

    public class WhenDeleteingRelationshipFromDictionaryWithNoWriteAccess : RelationshipControllerTestContext
    {
        public WhenDeleteingRelationshipFromDictionaryWithNoWriteAccess()
        {
            UserHelper.WithUserId("33");
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = "32" });
            FakeQueryProcessor.SetupResultFor<RelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Delete(34).Result;
        }

        [Fact]
        public void ShouldReturnUnauthorised()
        {
            Assert.IsType<UnauthorizedResult>(Result);
        }
    }

    public abstract class RelationshipControllerTestContext
    {
        protected readonly RelationshipController Controller;
        protected readonly Mock<IAmACommandProcessor> MockCommandProcessor;
        protected readonly FakeRelationshipRenderer FakeRelationshipRenderer;
        protected readonly FakeQueryProcessor FakeQueryProcessor;
        protected readonly FakeUserHelper UserHelper;
        protected IActionResult Result;

        protected RelationshipControllerTestContext()
        {
            Mapper.Initialize(c => c.AddProfile(new MappingProfile()));

            MockCommandProcessor = new Mock<IAmACommandProcessor>();
            FakeRelationshipRenderer = new FakeRelationshipRenderer();
            FakeQueryProcessor = new FakeQueryProcessor();
            UserHelper = new FakeUserHelper();
            Controller = new RelationshipController(MockCommandProcessor.Object, FakeQueryProcessor, UserHelper, FakeRelationshipRenderer);
        }
    }
}