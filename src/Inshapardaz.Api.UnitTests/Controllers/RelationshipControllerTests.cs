using System;
using System.Collections.Generic;
using AutoMapper;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.UnitTests.Fakes;
using Inshapardaz.Api.UnitTests.Fakes.Helpers;
using Inshapardaz.Api.UnitTests.Fakes.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paramore.Brighter;
using Xunit;

namespace Inshapardaz.Api.UnitTests.Controllers
{
    public class WhenGettingRelationshipsByWord : RelationshipControllerTestContext
    {
        public WhenGettingRelationshipsByWord()
        {
            FakeQueryProcessor.SetupResultFor<GetRelationshipsByWordQuery, IEnumerable<WordRelation>>(new List<WordRelation> { new WordRelation() });
            FakeRelationshipRenderer.WithView(new RelationshipView());
            Result = Controller.GetRelationshipsForWord(1, 9).Result;
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
            FakeQueryProcessor.SetupResultFor<GetRelationshipsByWordQuery, IEnumerable<WordRelation>>(new List<WordRelation>());
            Result = Controller.GetRelationshipsForWord(1, 9).Result;
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
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            FakeRelationshipRenderer.WithView(new RelationshipView());
            Result = Controller.Get(1, 23).Result;
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
            Result = Controller.Get(1, 23).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            Result = Controller.Get(1, 23).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(new Word());
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            FakeRelationshipRenderer.WithView(new RelationshipView());
            FakeRelationshipRenderer.WithLink("self", new System.Uri("http://link.test/123"));

            Result = Controller.Post(1, 23, new RelationshipView()).Result;
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
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(w => w.WordId == 12, new Word());
            Result = Controller.Post(1, 23, new RelationshipView { RelatedWordId = 12 }).Result;
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
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(w => w.WordId == 23, new Word());
            Result = Controller.Post(1, 23, new RelationshipView { RelatedWordId = 12 }).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(new Word());

            Result = Controller.Post(1, 23, new RelationshipView()).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(x => x.WordId == 23, new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetWordByIdQuery, Word>(new Word());

            Result = Controller.Post(1, 23, new RelationshipView { RelatedWordId = 45 }).Result;
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
            Result = Controller.Post(1, 23, null).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(1,  32, new RelationshipView()).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Put(1, 32, new RelationshipView()).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(q => q.RelationshipId == 32, new WordRelation());
            Result = Controller.Put(1, 32, new RelationshipView { RelatedWordId = 344 }).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(1, 32, new RelationshipView()).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(q => q.WordId == 32, new Dictionary { Id = 3, UserId = userId });
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(q => q.WordId == 43, new Dictionary { Id = 4, UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Put(1, 32, new RelationshipView { SourceWordId = 32, RelatedWordId = 43 }).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Delete(1, 34).Result;
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
            var userId = Guid.NewGuid();
            UserHelper.WithUserId(userId);
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = userId });
            Result = Controller.Delete(1, 34).Result;
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
            UserHelper.WithUserId(Guid.NewGuid());
            FakeQueryProcessor.SetupResultFor<DictionaryByWordIdQuery, Dictionary>(new Dictionary { UserId = Guid.NewGuid() });
            FakeQueryProcessor.SetupResultFor<GetRelationshipByIdQuery, WordRelation>(new WordRelation());
            Result = Controller.Delete(1, 34).Result;
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
            Controller = new RelationshipController(MockCommandProcessor.Object);
        }
    }
}